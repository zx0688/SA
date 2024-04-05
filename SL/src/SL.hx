import GameMeta;
import seedyrng.Random;
import GameData;
import haxe.macro.Context;

using SL.CSExtension;

#if cs
import cs.system.collections.generic.Dictionary_2;
import cs.system.collections.generic.List_1;
import cs.NativeArray;
#else
typedef NativeArray<T> = Array<T>;
typedef List_1<T> = Array<T>;
typedef Dictionary_2<T, K> = Map<T, K>;
#end

class SL {
	private static var random:Random;

	// random based on shared timestamp
	private static function GetRandomInstance():Random {
		if (random == null)
			random = new Random();
		return random;
	}

	private static var tempArray:Array<CardMeta> = null;

	// private static function GetTempArrayUnsafe():Array<CardMeta> {
	// 	if (tempArray == null)
	// 		tempArray = new Array<CardMeta>();
	// 	else
	// 		tempArray.splice(0, tempArray.length);
	// 	return tempArray;
	// }

	static function main() {}

	public static function GetPriceReroll(timeleft:Int, meta:GameMeta):Int {
		return cast Math.ceil(cast(timeleft, Float) / (meta.Config.DurationReroll / -meta.Config.PriceReroll[0].Count));
	}

	public static function Left(time:Int, start:Int, duration:Int):Int {
		return (duration + start) - time;
	}

	public static function CreateProfile(meta:GameMeta, timestamp:Int, random:Random):ProfileData {
		// there should be getting profile from Mongo database on server side
		var profile:ProfileData = new ProfileData();
		profile.Cards = new Dictionary_2<String, CardData>();
		profile.Cooldown = 0;
		profile.CurrentLocation = "28354825";
		profile.Deck = new List_1();
		profile.Accept = new Dictionary_2<String, GameRequest>();
		profile.OpenedLocations = new List_1();
		profile.Rid = 0;
		profile.Sid = 0;
		profile.SwipeCount = 0;
		profile.Items = new Dictionary_2<String, ItemData>();
		profile.Items.getOrCreate("6", f -> new ItemData("6", 998));
		// profile.Items.getOrCreate("3", f -> new ItemData("3", 998));
		// profile.Items.getOrCreate("13", f -> new ItemData("13", 10));
		// profile.Items.getOrCreate("5", f -> new ItemData("5", 2));
		// profile.Items.getOrCreate("11", f -> new ItemData("13", 10));
		// profile.Items.getOrCreate("62", f -> new ItemData("62", 10));

		profile.Items.getOrCreate("11", f -> new ItemData("11", 10));
		profile.Items.getOrCreate("42", f -> new ItemData("42", 1));
		profile.Items.getOrCreate("38", f -> new ItemData("38", 6));

		profile.Skills = new List_1();

		profile.Skills.push(null);
		profile.Skills.push(null);
		profile.Skills.push(null);
		profile.Skills.push(null);

		profile.LastChange = timestamp;
		profile.Created = timestamp;
		profile.ActiveQuests = new List_1();
		profile.Rerolls = 0;

		profile.RewardEvents = new List_1();
		profile.QuestEvent = null;

		// profile.Deck.push("28366105");
		// profile.Deck.push("28366091");

		// profile.Deck.push("28362199");

		// profile.Deck.push("28362200");

		// торговая улица
		// profile.Deck.push("27905697");
		// грибная роща
		// profile.Deck.push("28390976");
		// кристалические пещеры
		profile.Deck.push("27901221");

		// средний город
		// profile.Deck.push("27905696");

		CreateLeftRight(meta.Cards.tryGet(GetCurrentCard(profile)).Next, meta, profile, random);

		return profile;
	}

	public static function Change(request:GameRequest, meta:GameMeta, profile:ProfileData, timestamp:Int, response:GameResponse):Void {
		if (request.Timestamp > timestamp) {
			response.Error = "request can't be created later than the current time";
			return;
		}
		if (request.Timestamp < profile.LastChange) {
			response.Error = "request can't be created earlier than the last executed request";
			return;
		}
		if (request.Rid != profile.Rid) {
			response.Error = "request should have valid rid " + profile.Rid;
			return;
		}

		// just temp events for client, we can delete these safty
		profile.RewardEvents = new List_1();
		profile.QuestEvent = null;

		switch (request.Type) {
			case TriggerMeta.CHANGE_LOCATION:
				if (profile.CurrentLocation != request.Id) {
					response.Error = "error current location " + request.Id;
					return;
				}
				if (!profile.OpenedLocations.contains(request.Hash)) {
					response.Error = "error next location " + request.Value;
					return;
				}
				var location:CardMeta = meta.Locations.tryGet(request.Hash);
				// if (!CheckCondition(location.Act.Con, null, profile)) {
				// 	response.Error = "error loc condition " + request.Value;
				// 	return;
				// }
				profile.CurrentLocation = location.Id;

			case TriggerMeta.SWIPE:
				var deck:List_1<String> = profile.Deck;
				if (deck.getCount() == 0) {
					response.Error = "error deck is empty " + request.Hash;
					return;
				}
				if (GetCurrentCard(profile) != request.Hash) {
					response.Error = "error current card " + request.Hash;
					return;
				}
				if (request.Id == null && (profile.Left != null || profile.Right != null)) {
					response.Error = "id should not be empty ";
					return;
				}
				if (request.Id != null && profile.Left == null && profile.Right == null) {
					response.Error = "id should be empty" + request.Id;
					return;
				}
				if ((profile.Left != null && request.Id != profile.Left.Id)
					&& ((profile.Right != null && request.Id != profile.Right.Id) || profile.Right == null)) {
					response.Error = "id should match with choice" + request.Id;
					return;
				}

				var swipedCard:CardMeta = meta.Cards.tryGet(request.Hash);

				if ((swipedCard.Descs != null && profile.DialogIndex >= swipedCard.Descs.length)
					|| (swipedCard.Descs == null && profile.DialogIndex > 0)) {
					response.Error = "dialog index is out" + request.Id;
					return;
				}

				// Apply card
				var card:CardData = profile.Cards.getOrCreate(request.Hash, f -> new CardData(request.Hash));
				card.CT++;
				card.CR++;
				card.Choice = request.Id;
				profile.SwipeCount++;

				// only description card
				if (swipedCard.Descs != null && profile.DialogIndex > 0) {
					profile.DialogIndex--;
					return;
				}

				random = GetRandomInstance();
				random.setStringSeed(Std.string(request.Timestamp));

				deck.pop();

				var next:CardNextInfo = null;
				if (profile.Left != null && request.Id == profile.Left.Id)
					next = profile.Left;
				else if (profile.Right != null && request.Id == profile.Right.Id)
					next = profile.Right;

				if (next != null && next.Next != null) {
					deck.push(next.Next);
				}

				if (swipedCard.Under != null) {
					for (o in swipedCard.Under) {
						var oc:CardMeta = meta.Cards.tryGet(o.Id);
						if (CheckCard(oc, o, meta, profile, random))
							deck.push(oc.Id);
					}
				}

				if (request.Id != null) {
					var nextCard:CardMeta = meta.Cards.tryGet(request.Id);
					RecursiveCreateDeck(nextCard, deck, meta, profile, random);
				}

				if (swipedCard.Type == CardMeta.TYPE_CARD && profile.ActiveQuests.getCount() > 0) {
					// Quests
					var rem:List_1<String> = new List_1<String>();
					for (qID in profile.ActiveQuests) {
						var qm:CardMeta = meta.Cards.tryGet(qID);
						if ((qm.ST == null
							|| (qm.ST != null
								&& (qm.ST[0].Type == TriggerMeta.ALWAYS || qm.ST.find(qms -> qms.Id == swipedCard.Id) != null)))
							&& CheckAnyCondition(qm.SC, meta, profile, random)) {
							var qp:CardData = profile.Cards.tryGet(qID);
							qp.Value = CardMeta.QUEST_SUCCESS;
							rem.push(qID);
							deck.push(qID);
						}
						// } else if (qm.FT != null
						// 	&& (qm.FT[0].Type == TriggerMeta.ALWAYS || qm.FT.find(qms -> qms.Id == swipedCard.Id) != null)
						// 	&& CheckCondition(qm.FC, meta, profile, random)) {
						// 	var qp:CardData = profile.Cards.tryGet(qID);
						// 	qp.Value = CardMeta.QUEST_FAIL;
						// 	profile.QuestEvent = qp.Id;
						// 	profile.ActiveQuests.removeItem(qID);
						// 	ApplyReward(qm.FR, meta, profile, random);
						// 	deck.push(qID);
						// }
					}
					for (qID in rem)
						profile.ActiveQuests.removeItem(qID);
				}

				profile.Left = null;
				profile.Right = null;
				while (profile.Deck.getCount() > 0) {
					var nextCard:CardMeta = meta.Cards.tryGet(GetCurrentCard(profile));
					if (!CheckNext(nextCard, null, meta, profile, random)) {
						profile.Deck.removeItem(nextCard.Id);
						continue;
					}

					if (nextCard.Delete) {
						profile.Deck = new List_1();
						profile.Deck.push(nextCard.Id);
					}

					profile.DialogIndex = nextCard.Descs != null && nextCard.Descs.length > 0 ? nextCard.Descs.length - 1 : 0;

					ApplyReward(GetRewardByCondition(nextCard.Reward, nextCard.Con, meta, profile, random, response), meta, profile, random);

					if (nextCard.Type == CardMeta.TYPE_QUEST) {
						var card:CardData = profile.Cards.tryGet(nextCard.Id);
						if (card == null) {
							card = new CardData(nextCard.Id);
							card.Value = CardMeta.QUEST_ACTIVE;
							profile.ActiveQuests.push(nextCard.Id);
							profile.Cards.set(nextCard.Id, card);
						} else if (card.Value == CardMeta.QUEST_SUCCESS) {
							ApplyReward(GetRewardByCondition(nextCard.SR, nextCard.SC, meta, profile, random, response), meta, profile, random);
						}
						profile.QuestEvent = nextCard.Id;
					} else {
						if (nextCard.Next != null) {
							var next:NativeArray<TriggerMeta> = nextCard.Next;
							CreateLeftRight(next, meta, profile, random);
						}
					}

					break;
				}
				if (profile.Deck.getCount() == 0) {
					profile.Cooldown = timestamp;
				}

			case TriggerMeta.REROLL:
				if (profile.Deck.getCount() > 0) {
					response.Error = "cards are available";
					return;
				}
				if (profile.Cooldown == 0) {
					response.Error = "cooldown should not be 0";
					return;
				}
				var left:Int = Left(request.Timestamp, profile.Cooldown, meta.Config.DurationReroll);
				var price:Int = left > 0 ? GetPriceReroll(left, meta) : 0;

				var id:String = meta.Config.PriceReroll[0].Id;
				var i:ItemData = profile.Items.getOrCreate(id, t -> new ItemData(id, 0));
				if (i.Count < price) {
					response.Error = "not enough items for reroll";
					return;
				}
				i.Count -= price;
				i.Count = i.Count < 0 ? 0 : i.Count;
				profile.Deck.push(meta.Locations.tryGet(profile.CurrentLocation).Over[0].Id);
				profile.Cooldown = 0;
				profile.Rerolls++;
				var r:RewardMeta = Type.createEmptyInstance(RewardMeta);
				r.Id = id;
				r.Type = ConditionMeta.ITEM;
				r.Count = -price;
				profile.RewardEvents.push(r);

				for (key in profile.Cards.GetKeys()) {
					var qd:CardData = profile.Cards.tryGet(cast key);
					qd.CR = 0;
				}

				CreateLeftRight(meta.Cards.tryGet(profile.Deck[0]).Next, meta, profile, random);

			case TriggerMeta.START_GAME:

			case TriggerMeta.EVENT:
				if (request.Hash == null) {
					response.Error = "an event trigger should have a hash";
					return;
				}
				var accepts:Dictionary_2<String, GameRequest> = profile.Accept;
				var r:GameRequest = accepts.tryGet(request.Hash);
				if (r == null) {
					response.Error = "profile should have an event with the same hash";
					return;
				}

				// change profile
				var items:Dictionary_2<String, ItemData> = profile.Items;
				var i:ItemData = items.getOrCreate(r.Id, f -> new ItemData(r.Id, 0));
				i.Count += r.Value;
				// items[r.Id] = i;
				accepts.remove(request.Hash);
			default:
				response.Error = "unexpected request";
				return;
		}

		profile.LastChange = request.Timestamp;
		profile.Rid += 1;
	}

	public static function CheckCard(cardMeta:CardMeta, triggerMeta:TriggerMeta, data:GameMeta, profile:ProfileData, random:Random):Bool {
		if (cardMeta == null)
			return false;

		if (cardMeta.CT > 0 || cardMeta.CR > 0) {
			var cardData:CardData = profile.Cards.tryGet(cardMeta.Id);
			if (cardData != null && ((cardMeta.CT != 0 && cardData.CT >= cardMeta.CT) || (cardMeta.CR != 0 && cardData.CR >= cardMeta.CR)))
				return false;
		}

		if (!CheckAnyCondition(cardMeta.Con, data, profile, random))
			return false;

		if (triggerMeta != null) {
			if (triggerMeta.Chance > 0 && random.randomInt(0, 100) > triggerMeta.Chance)
				return false;
			// if (triggerMeta.Value2 != null) {
			// 	var cm:CardMeta = data.Cards.tryGet(triggerMeta.Value2);
			// 	if (CheckCard(cm, null, data, profile, random))
			// 		return false;
			// }
		}

		if (cardMeta.Next != null && cardMeta.Next.length > 0) {
			var f:Bool = false;
			for (c in cardMeta.Next) {
				var cm:CardMeta = data.Cards.tryGet(c.Id);
				if (CheckCard(cm, c, data, profile, random)) {
					f = true;
					break;
				}
			}
			if (f == false)
				return false;
		}

		return true;
	}

	public static function CheckNext(cardMeta:CardMeta, triggerMeta:TriggerMeta, data:GameMeta, profile:ProfileData, random:Random):Bool {
		if (cardMeta.Next != null && cardMeta.Next.length > 0) {
			var f:Bool = false;
			for (c in cardMeta.Next) {
				// if (c.Id == "28440012")
				// continue;
				var cm:CardMeta = data.Cards.tryGet(c.Id);
				if (CheckCard(cm, c, data, profile, random)) {
					f = true;
					break;
				}
			}
			if (f == false)
				return false;
		}

		return true;
	}

	public static function RecursiveCreateDeck(nextCard:CardMeta, deck:List_1<String>, meta:GameMeta, profile:ProfileData, random:Random):Void {
		// var nextCard:CardMeta = meta.Cards.tryGet(request.Id);
		// if (nextCard.Type == CardMeta.TYPE_CARD || nextCard.Type == CardMeta.TYPE_QUEST) {
		// 	deck.push(nextCard.Id);
		// }

		deck.push(nextCard.Id);

		if (nextCard.Over == null)
			return;

		var over:NativeArray<TriggerMeta> = nextCard.Over;
		// var candidates:Array<CardMeta> = new Array<CardMeta>();
		for (o in over) {
			var oc:CardMeta = meta.Cards.tryGet(o.Id);
			if (CheckCard(oc, o, meta, profile, random)) {
				//	candidates.push(oc);
				RecursiveCreateDeck(oc, deck, meta, profile, random);
			}
		}
		// if (candidates.length > 0) {
		// 	tempArray = new Array<CardMeta>();
		// 	candidates.sort((a, b) -> a.Pri - b.Pri);
		// 	for (c in candidates) {
		// 		deck.push(c.Id);
		// 		tempArray.push(c);
		// 	}
		// 	for (c in tempArray)
		// 		RecursiveOver(c, deck, meta, profile, random);
		// }
	}

	public static function CheckReward(rewardMeta:RewardMeta, data:GameMeta, profile:ProfileData, random:Random):Bool {
		if (rewardMeta.Chance > 0 && random.randomInt(0, 100) > rewardMeta.Chance)
			return false;

		if (!CheckCondition(rewardMeta.Con, data, profile, random))
			return false;

		return true;
	}

	public static function CheckAnyCondition(con:Null<NativeArray<NativeArray<ConditionMeta>>>, data:GameMeta, profile:ProfileData, random:Random):Bool {
		if (con == null || con.length == 0)
			return true;
		for (c in con)
			if (CheckCondition(c, data, profile, random))
				return true;
		return false;
	}

	public static function CheckCondition(con:Null<NativeArray<ConditionMeta>>, data:GameMeta, profile:ProfileData, random:Random):Bool {
		if (con == null || con.length == 0)
			return true;

		for (c in con) {
			switch (c.Type) {
				case ConditionMeta.CARD_COND:
					var card:CardMeta = data.Cards.tryGet(c.Id);
					if (c.Invert == true && CheckCard(card, null, data, profile, random))
						return false;
					else if (c.Invert == false && !CheckCard(card, null, data, profile, random))
						return false;
				case ConditionMeta.CARD:
					var card:CardData = profile.Cards.tryGet(c.Id);
					if (card == null || card.CT < c.Count)
						return false;

				case ConditionMeta.ITEM:
					var item:ItemData = profile.Items.tryGet(c.Id);
					var count:Int = item != null ? item.Count : 0;

					switch (c.Sign) {
						case ">": if (!(count > c.Count)) return false;
						case "==": if (!(c.Count == count)) return false;
						case "<=": if (!(count <= c.Count)) return false;
						case ">=": if (!(count >= c.Count)) return false;
						case "<": if (!(count < c.Count)) return false;
						default:
							if (count == 0) return false;
					}
				default:
			}
		}
		return true;
	}

	public static function GetRewardByCondition(reward:NativeArray<NativeArray<RewardMeta>>, cond:NativeArray<NativeArray<ConditionMeta>>, meta:GameMeta,
			profile:ProfileData, random:Random, response:GameResponse):NativeArray<RewardMeta> {
		var rew:NativeArray<RewardMeta> = null;
		if (reward == null)
			return null;
		else if (cond == null || cond.length <= 1) {
			rew = reward[0];
		} else {
			var checkf = false;
			for (cc in 0...cond.length)
				if (CheckCondition(cond[cc], meta, profile, random)) {
					rew = reward[cc];
					checkf = true;
					break;
				}

			if (checkf == false) {
				response.Error = "reward shoud match with condition";
				return null;
			}
		}
		return rew;
	}

	private static function ApplyReward(reward:NativeArray<RewardMeta>, meta:GameMeta, profile:ProfileData, random:Random):Void {
		if (reward == null)
			return;
		for (r in reward) {
			if (!CheckReward(r, meta, profile, random))
				continue;
			switch (r.Type) {
				case ConditionMeta.ITEM:
					var i:ItemData = profile.Items.getOrCreate(r.Id, f -> new ItemData(r.Id, 0));
					i.Count += r.Count;
					i.Count = i.Count < 0 ? 0 : i.Count;
				case ConditionMeta.SKILL:
					var m:SkillMeta = meta.Skills.tryGet(r.Id);
					profile.Skills[cast m.Slot] = r.Id;
			}

			profile.RewardEvents.push(r);
		}
	}

	private static function GetCurrentCard(profile:ProfileData):String {
		return profile.Deck[profile.Deck.getCount() - 1];
	}

	public static function RecursiveGroup(candidates:Array<CardMeta>, nextDict:Dictionary_2<String, String>, nextCard:String, next:NativeArray<TriggerMeta>,
			meta:GameMeta, profile:ProfileData, random:Random):Void {
		for (n in next) {
			if (n.Type == CardMeta.TYPE_GROUP) {
				var group:GroupMeta = meta.Groups.tryGet(n.Id);
				RecursiveGroup(candidates, nextDict, nextCard, group.Cards, meta, profile, random);
			} else {
				var nc:CardMeta = meta.Cards.tryGet(n.Id);
				if (CheckCard(nc, n, meta, profile, random)) {
					candidates.push(nc);
					nextDict.set(nc.Id, nextCard);
				}
			}
		}
	}

	private static function CreateLeftRight(next:NativeArray<TriggerMeta>, meta:GameMeta, profile:ProfileData, random:Random):Void {
		var candidates:Array<CardMeta> = new Array<CardMeta>();
		var nextDict:Dictionary_2<String, String> = new Dictionary_2<String, String>();

		for (n in next) {
			if (n.Type == CardMeta.TYPE_GROUP) {
				var group:GroupMeta = meta.Groups.tryGet(n.Id);
				RecursiveGroup(candidates, nextDict, n.Next, group.Cards, meta, profile, random);
			} else {
				var nc:CardMeta = meta.Cards.tryGet(n.Id);
				if (CheckCard(nc, n, meta, profile, random)) {
					candidates.push(nc);
					nextDict.set(nc.Id, n.Next);
				}
			}
		}

		if (candidates.length == 0) {
			profile.Left = null;
			profile.Right = null;
			return;
		} else if (candidates.length == 1) {
			profile.Left = new CardNextInfo(candidates[0].Id, nextDict.tryGet(candidates[0].Id));
			profile.Right = null;
			return;
		} else if (candidates.length == 2) {
			profile.Left = new CardNextInfo(candidates[0].Id, nextDict.tryGet(candidates[0].Id));
			profile.Right = new CardNextInfo(candidates[1].Id, nextDict.tryGet(candidates[1].Id));
			return;
		}
		candidates.sort((a, b) -> b.Pri - a.Pri);
		var first:Int = candidates[0].Pri;
		var filtered:Array<CardMeta> = candidates.filter(c -> c.Pri == first);
		if (filtered.length == 1) {
			profile.Left = new CardNextInfo(filtered[0].Id, nextDict.tryGet(filtered[0].Id));
			candidates.remove(filtered[0]);
		} else {
			var d:CardMeta = filtered[random.randomInt(0, filtered.length - 1)];
			profile.Left = new CardNextInfo(d.Id, nextDict.tryGet(d.Id));
			candidates.remove(d);
		}
		candidates.sort((a, b) -> b.Pri - a.Pri); // without left card
		var first:Int = candidates[0].Pri;
		var filtered:Array<CardMeta> = candidates.filter(c -> c.Pri == first);
		if (filtered.length == 1) {
			profile.Right = new CardNextInfo(filtered[0].Id, nextDict.tryGet(filtered[0].Id));
		} else {
			var _id = filtered[random.randomInt(0, filtered.length - 1)].Id;

			profile.Right = new CardNextInfo(_id, nextDict.tryGet(_id));
		}
	}
}

#if cs
class CSExtension {
	@:generic public static function find<T>(_this:NativeArray<T>, f:T->Bool):Null<T> {
		for (c in _this) {
			if (f(c))
				return c;
		}
		return null;
	}

	public static function iterator<T>(hashSet:cs.system.collections.generic.IEnumerable_1<T>):Iterator<T> {
		return new IEnumerableIterator(hashSet);
	}

	@:generic public static function getCount<T>(_this:cs.system.collections.generic.List_1<T>):Int {
		return _this.Count;
	}

	@:generic public static function GetKeys<K, T>(_this:cs.system.collections.generic.Dictionary_2<K, T>):Iterator<K> {
		return _this.Keys.iterator();
	}

	@:generic public static function push<T>(_this:cs.system.collections.generic.List_1<T>, x:T):Void {
		_this.Add(x);
	}

	@:generic public static function removeItem<T>(_this:cs.system.collections.generic.List_1<T>, x:T):Void {
		_this.Remove(x);
	}

	@:generic public static function contains<T>(_this:cs.system.collections.generic.List_1<T>, x:T):Bool {
		return _this.Contains(x);
	}

	public static function pop<T>(_this:cs.system.collections.generic.List_1<T>):T {
		var i:T = _this[0];
		_this.RemoveAt(_this.Count - 1);
		return i;
	}

	public static function sort<T>(_this:cs.system.collections.generic.List_1<T>, f:T->T->Int):Void {
		_this.Sort(f);
	}

	@:generic public static function tryGet<K, T>(_this:cs.system.collections.generic.Dictionary_2<K, T>, key:K):Null<T> {
		var i:T = null;
		if (_this.TryGetValue(key, i)) {
			return i;
		}
		return null;
	}

	@:generic public static function getOrCreate<K, T>(_this:cs.system.collections.generic.Dictionary_2<K, T>, key:K, f:T->T):T {
		var i:T = null;
		if (!_this.TryGetValue(key, i)) {
			i = f(i);
			_this.set_Item(key, i);
		}
		return i;
	}

	@:generic public static function set<K, T>(_this:cs.system.collections.generic.Dictionary_2<K, T>, key:K, value:T):Void {
		return _this.set_Item(key, value);
	}

	@:generic public static function remove<K, T>(_this:cs.system.collections.generic.Dictionary_2<K, T>, key:K):Bool {
		return _this.Remove(key);
	}
}

class IEnumerableIterator<T> {
	var enumerator:cs.system.collections.generic.IEnumerator_1<T>;
	var hasNextBool = false;
	var nextCalled = false;

	public function new(enumerable:cs.system.collections.generic.IEnumerable_1<T>) {
		enumerator = enumerable.GetEnumerator();
	}

	public function hasNext():Bool {
		if (!nextCalled) {
			hasNextBool = enumerator.MoveNext();
			nextCalled = true;
		}
		return hasNextBool;
	}

	public function next():T {
		hasNext();
		nextCalled = false;
		return enumerator.Current;
	}
}
#else
class CSExtension {
	@:generic public static function removeItem<T>(_this:Array<T>, x:T):Void {
		_this.remove(x);
	}

	@:generic public static function GetKeys<K, T>(_this:Map<K, T>):Iterator<K> {
		return _this.keys();
	}

	@:generic public static function find<T>(_this:NativeArray<T>, f:T->Bool):Null<T> {
		return Lambda.find(_this, f);
	}

	@:generic public static function getCount<T>(_this:Array<T>):Int {
		return _this.length;
	}

	@:generic public static function tryGet<K, T>(_this:Map<K, T>, key:K):Null<T> {
		var i:T = _this.get(key);
		if (i != null) {
			return i;
		}
		return null;
	}

	@:generic public static function getOrCreate<K, T>(_this:Map<K, T>, key:K, f:T->T):T {
		var i:T = _this.get(key);
		if (i == null) {
			i = f(i);
			_this.set(key, i);
		}
		return i;
	}
}
#end
