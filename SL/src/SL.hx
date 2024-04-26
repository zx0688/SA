import haxe.Int64;
import haxe.Json;
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

	public static function Left(time:Int64, start:Int64, duration:Int):Int {
		return Int64.toInt((haxe.Int64.ofInt(duration) + start) - time);
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
		profile.DialogIndex = 0;
		profile.Rid = 0;
		profile.Sid = 0;
		profile.SwipeCount = 0;
		profile.Items = new Dictionary_2<String, ItemData>();
		profile.Items.getOrCreate("6", f -> new ItemData("6", 998));
		// profile.Items.getOrCreate("3", f -> new ItemData("3", 998));
		// profile.Items.getOrCreate("13", f -> new ItemData("13", 10));
		// profile.Items.getOrCreate("5", f -> new ItemData("5", 2));
		// profile.Items.getOrCreate("11", f -> new ItemData("11", 10));
		// profile.Items.getOrCreate("62", f -> new ItemData("62", 10));
		// for (i in 1...64)
		// 	profile.Items.getOrCreate(i + "", f -> new ItemData(i + "", 10));
		// 		profile.Items.getOrCreate("55", f -> new ItemData("55", 10));
		// 		profile.Items.getOrCreate("42", f -> new ItemData("42", 1));
		// 		profile.Items.getOrCreate("34", f -> new ItemData("34", 10));
		// 		profile.Items.getOrCreate("49", f -> new ItemData("49", 10));
		//
		// 		profile.Items.getOrCreate("20", f -> new ItemData("20", 10));
		// 		profile.Items.getOrCreate("64", f -> new ItemData("64", 10));
		//
		// 		profile.Items.getOrCreate("35", f -> new ItemData("35", 10));
		// profile.Items.getOrCreate("64", f -> new ItemData("64", 10));

		profile.Skills = new List_1();

		profile.Skills.push(null);
		profile.Skills.push(null);
		profile.Skills.push(null);
		profile.Skills.push(null);

		profile.LastChange = timestamp;
		profile.Created = timestamp;
		profile.ActiveQuests = new List_1();
		profile.History = new List_1();
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
		profile.Deck.push("27901213");

		// средний город
		// profile.Deck.push("27905696");

		CreateLeftRight(meta.Cards.tryGet(GetCurrentCard(profile)).Next, meta, profile, random);

		return profile;
	}

	private static function SetRandomSeedByTime(timestamp64:Int64):Void {
		random = GetRandomInstance();
		var seed:String = Std.string(timestamp64);
		var newSeed:String = "";
		for (i in 1...7)
			newSeed += seed.charAt(seed.length - i);
		random.setStringSeed(newSeed);
	}

	public static function Change(request:GameRequest, meta:GameMeta, profile:ProfileData, timestamp:Int, response:GameResponse):Void {
		var requestTimestamp:Int = Int64.toInt(request.Timestamp / 1000);
		if (requestTimestamp > timestamp + 1) {
			response.Error = "request can't be created later than the current time";
			return;
		}
		if (requestTimestamp + 60 < timestamp) {
			response.Error = "request is expired";
			return;
		}
		if (requestTimestamp < profile.LastChange) {
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

				if ((swipedCard.Descs != null && profile.DialogIndex > swipedCard.Descs.length)
					|| (swipedCard.Descs == null && profile.DialogIndex > 0)) {
					response.Error = "dialog index is out " + request.Hash;
					return;
				}

				profile.SwipeCount++;

				// only description card
				if (swipedCard.Descs != null && swipedCard.Next != null && profile.DialogIndex < swipedCard.Descs.length) {
					var card:CardData = profile.Cards.getOrCreate(request.Hash, f -> new CardData(request.Hash));
					if (card.CT == 0) {
						profile.DialogIndex++;
						if (CheckCard(swipedCard, meta, profile, random))
							return;
					}
				} else if (swipedCard.Descs != null && profile.DialogIndex < swipedCard.Descs.length - 1) {
					profile.DialogIndex++;
					return;
				}

				// Apply card
				var card:CardData = profile.Cards.getOrCreate(request.Hash, f -> new CardData(request.Hash));
				card.CT++;
				card.CR++;
				card.Choice = request.Id;
				profile.History.push(card.Id);
				profile.DialogIndex = 0;

				SetRandomSeedByTime(requestTimestamp);

				deck.pop();

				var nextCardInfo:CardNextInfo = null;
				if (profile.Left != null && request.Id == profile.Left.Id)
					nextCardInfo = profile.Left;
				else if (profile.Right != null && request.Id == profile.Right.Id)
					nextCardInfo = profile.Right;
				if (nextCardInfo != null && nextCardInfo.Next != null) {
					var nextCard:CardMeta = meta.Cards.tryGet(nextCardInfo.Next);
					RecursiveCreateDeck(nextCard, deck, meta, profile, random);
				}

				profile.Left = null;
				profile.Right = null;

				// build current deck
				if (request.Id != null) {
					var nextCard:CardMeta = meta.Cards.tryGet(request.Id);
					RecursiveCreateDeck(nextCard, deck, meta, profile, random);
				}

				// if (swipedCard.Type == CardMeta.TYPE_CARD && profile.ActiveQuests.getCount() > 0) {

				// 	// Quests
				// 	var rem:List_1<String> = new List_1<String>();
				// 	for (qID in profile.ActiveQuests) {
				// 		var qm:CardMeta = meta.Cards.tryGet(qID);
				// 		if ((qm.ST == null
				// 			|| (qm.ST != null
				// 				&& (qm.ST[0].Type == TriggerMeta.ALWAYS || qm.ST.find(qms -> qms.Id == swipedCard.Id) != null)))
				// 			&& CheckAnyCondition(qm.SC, meta, profile, random)) {
				// 			var qp:CardData = profile.Cards.tryGet(qID);
				// 			qp.Value = CardMeta.QUEST_SUCCESS;
				// 			rem.push(qID);
				// 			deck.push(qID);
				// 		}
				// 		// } else if (qm.FT != null
				// 		// 	&& (qm.FT[0].Type == TriggerMeta.ALWAYS || qm.FT.find(qms -> qms.Id == swipedCard.Id) != null)
				// 		// 	&& CheckCondition(qm.FC, meta, profile, random)) {
				// 		// 	var qp:CardData = profile.Cards.tryGet(qID);
				// 		// 	qp.Value = CardMeta.QUEST_FAIL;
				// 		// 	profile.QuestEvent = qp.Id;
				// 		// 	profile.ActiveQuests.removeItem(qID);
				// 		// 	ApplyReward(qm.FR, meta, profile, random);
				// 		// 	deck.push(qID);
				// 		// }
				// 	}
				// 	for (qID in rem)
				// 		profile.ActiveQuests.removeItem(qID);
				// }
				CheckAndClearDeck(meta, profile, random);

				if (profile.Deck.getCount() > 0) {
					var currentCard:CardMeta = meta.Cards.tryGet(GetCurrentCard(profile));

					ApplyReward(GetRewardByCondition(currentCard.Reward, currentCard.Con, meta, profile, random), meta, profile, random);

					if (currentCard.Type == CardMeta.TYPE_QUEST) {
						var card:CardData = profile.Cards.tryGet(currentCard.Id);
						if (card == null) {
							card = new CardData(currentCard.Id);
							card.Value = CardMeta.QUEST_ACTIVE;
							profile.ActiveQuests.push(currentCard.Id);
							profile.Cards.set(currentCard.Id, card);
						} else if (card.Value == CardMeta.QUEST_SUCCESS) {
							ApplyReward(GetRewardByCondition(currentCard.SR, currentCard.SC, meta, profile, random), meta, profile, random);
						}
						profile.QuestEvent = currentCard.Id;
					} else {
						if (currentCard.Next != null) {
							CreateLeftRight(currentCard.Next, meta, profile, random);
						}
					}
				} else {
					profile.Cooldown = requestTimestamp;
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
				var left:Int = Left(requestTimestamp, profile.Cooldown, meta.Config.DurationReroll);
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

				for (c in profile.History)
					profile.Cards.tryGet(c).CR = 0;

				profile.History = new List_1();

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

		profile.LastChange = requestTimestamp;
		profile.Rid += 1;
	}

	private static function CheckAndClearDeck(meta:GameMeta, profile:ProfileData, random:Random) {
		while (profile.Deck.getCount() > 0) {
			var currentCard:CardMeta = meta.Cards.tryGet(GetCurrentCard(profile));
			if (!CheckCard(currentCard, meta, profile, random)) {
				profile.Deck.RemoveAt(profile.Deck.getCount() - 1);
				continue;
			}
			break;
		}
	}

	public static function CheckCardTrigger(cardMeta:CardMeta, trigger:TriggerMeta, meta:GameMeta, profile:ProfileData, random:Random):Bool {
		if (cardMeta.CT > 0 || cardMeta.CR > 0) {
			var cardData:CardData = profile.Cards.tryGet(cardMeta.Id);
			if (cardData != null && ((cardMeta.CT != 0 && cardData.CT >= cardMeta.CT) || (cardMeta.CR != 0 && cardData.CR >= cardMeta.CR)))
				return false;
		}

		if (trigger != null) {
			if (trigger.Chance > 0 && random.randomInt(0, 100) > trigger.Chance)
				return false;
		}

		return true;
	}

	public static function RecursiveCreateDeck(currentCard:CardMeta, deck:List_1<String>, meta:GameMeta, profile:ProfileData, random:Random):Void {
		if (currentCard.Cut) {
			profile.Deck = new List_1();
			deck = profile.Deck;
		}

		if (currentCard.Under != null) {
			var candidates:List_1<TriggerMeta> = new List_1();
			for (o in currentCard.Under) {
				for (i in 0...(o.Count > 0 ? o.Count : 1))
					candidates.push(o);
			}
			if (currentCard.Shake)
				Shake(candidates, random);

			for (o in candidates) {
				var card:CardMeta = meta.Cards.tryGet(o.Id);
				if (CheckCardTrigger(card, o, meta, profile, random))
					RecursiveCreateDeck(card, deck, meta, profile, random);
			}
		}

		if (currentCard.Descs != null || currentCard.Next != null) {
			deck.push(currentCard.Id);
		}

		if (currentCard.Over != null) {
			var candidates:List_1<TriggerMeta> = new List_1();
			for (o in currentCard.Over) {
				for (i in 0...(o.Count > 0 ? o.Count : 1))
					candidates.push(o);
			}
			if (currentCard.Shake)
				Shake(candidates, random);

			for (o in candidates) {
				var card:CardMeta = meta.Cards.tryGet(o.Id);
				if (CheckCardTrigger(card, o, meta, profile, random))
					RecursiveCreateDeck(card, deck, meta, profile, random);
			}
		}
	}

	public static function CheckReward(rewardMeta:RewardMeta, data:GameMeta, profile:ProfileData, random:Random):Bool {
		if (rewardMeta.Chance > 0 && random.randomInt(0, 100) > rewardMeta.Chance)
			return false;

		if (!CheckCondition(rewardMeta.Con, data, profile, random))
			return false;

		return true;
	}

	public static function CheckCard(card:CardMeta, meta:GameMeta, profile:ProfileData, random:Random):Bool {
		if (!CheckCardTrigger(card, null, meta, profile, random))
			return false;

		if (card.Next != null && (card.Descs == null || profile.DialogIndex > 0)) {
			var hasNext:Bool = false;
			for (c in card.Next) {
				var cm:CardMeta = meta.Cards.tryGet(c.Id);
				if (CheckCard(cm, meta, profile, random)) {
					hasNext = true;
					break;
				}
			}
			if (hasNext == false)
				return false;
		}
		if (card.Con != null && card.Con.length > 0) {
			var match:Bool = false;
			for (c in card.Con) {
				if (CheckCondition(c, meta, profile, random)) {
					match = true;
					break;
				}
			}
			if (match == false)
				return false;
		}

		return true;
	}

	public static function CheckCondition(con:Null<NativeArray<ConditionMeta>>, meta:GameMeta, profile:ProfileData, random:Random):Bool {
		if (con == null || con.length == 0)
			return true;

		for (c in con) {
			switch (c.Type) {
				case ConditionMeta.CARD:
					var card:CardData = profile.Cards.tryGet(c.Id);
					var count:Int = card != null ? card.CT : 0;
					switch (c.Sign) {
						case ">": if (!(count > c.Count)) return false;
						case "==": if (!(c.Count == count)) return false;
						case "<=": if (!(count <= c.Count)) return false;
						case ">=": if (!(count >= c.Count)) return false;
						case "<": if (!(count < c.Count)) return false;
						default:
							if (count == 0) return false;
					}
				// if (c.Invert == true && CheckCard(card, meta, profile, random))
				// 	return false;
				// else if (c.Invert == false && !CheckCard(card, meta, profile, random))
				// 	return false;
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
			profile:ProfileData, random:Random):NativeArray<RewardMeta> {
		var rew:NativeArray<RewardMeta> = null;
		if (reward == null)
			return null;
		else if (cond == null || cond.length <= 1) {
			rew = reward[0];
		} else {
			var hasReward = false;
			for (cc in 0...cond.length)
				if (CheckCondition(cond[cc], meta, profile, random)) {
					rew = reward[cc];
					hasReward = true;
					break;
				}

			if (hasReward == false)
				return null;
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

	public static function GetCurrentCard(profile:ProfileData):String {
		return profile.Deck[profile.Deck.getCount() - 1];
	}

	private static function RecursiveGroup(candidates:Array<CardMeta>, nextDict:Dictionary_2<String, String>, nextCard:String, next:NativeArray<TriggerMeta>,
			meta:GameMeta, profile:ProfileData, random:Random):Void {
		for (n in next) {
			if (n.Type == CardMeta.TYPE_GROUP) {
				var group:GroupMeta = meta.Groups.tryGet(n.Id);
				RecursiveGroup(candidates, nextDict, nextCard, group.Cards, meta, profile, random);
			} else {
				var nc:CardMeta = meta.Cards.tryGet(n.Id);
				if (CheckCard(nc, meta, profile, random)) {
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
				if (CheckCard(nc, meta, profile, random)) {
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

	private static function Shake(arr:List_1<TriggerMeta>, random:Random):Void {
		var n:Int = arr.getCount();
		for (j in 0...2)
			for (i in 0...n) {
				var randomIndex:Int = random.randomInt(0, n - 1);
				var temp:TriggerMeta = arr[i];
				arr[i] = arr[randomIndex];
				arr[randomIndex] = temp;
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

	@:generic public static function pop<T>(_this:cs.system.collections.generic.List_1<T>):T {
		var i:T = _this[0];
		_this.RemoveAt(_this.Count - 1);
		return i;
	}

	@:generic public static function sort<T>(_this:cs.system.collections.generic.List_1<T>, f:T->T->Int):Void {
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
	@:generic public static function RemoveAt<T>(_this:Array<T>, x:Int):Void {
		_this.splice(x, 1);
	}

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
