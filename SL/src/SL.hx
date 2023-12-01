import GameMeta;
import seedyrng.Random;
import GameData;
import haxe.macro.Context;

using SL.CSExtension;

#if cs
import cs.system.collections.generic.Dictionary_2;
import cs.system.collections.generic.List_1;
import cs.system.collections.generic.IComparer_1;
import cs.NativeArray;
#else
typedef NativeArray<T> = Array<T>;
typedef List_1<T> = Array<T>;
typedef Dictionary_2<T, K> = Map<T, K>;
#end

class SL {
	private static var random:Random;

	public static function GetRandomInstance():Random {
		if (random == null)
			random = new Random();
		return random;
	}

	private static var tempArray:Array<CardMeta> = null;

	private static function GetTempArray():Array<CardMeta> {
		if (tempArray == null)
			tempArray = new Array<CardMeta>();
		else
			tempArray.splice(0, tempArray.length);
		return tempArray;
	}

	static function main() {}

	public static function GetPriceReroll(timeleft:Int, meta:GameMeta):Int {
		return cast Math.floor(cast(timeleft, Float) / (meta.Config.DurationReroll / meta.Config.PriceReroll[0].Count));
	}

	public static function Left(time:Int, start:Int, duration:Int):Int {
		return (duration + start) - time;
	}

	public static function CreateProfile(meta:GameMeta, timestamp:Int, random:Random):ProfileData {
		random.setStringSeed(Std.string(timestamp));

		var profile:ProfileData = new ProfileData();
		profile.Cards = new Dictionary_2<String, CardData>();
		profile.Cooldown = 0;
		profile.CurrentLocation = "28354825";
		profile.Deck = new List_1();
		profile.Events = new Dictionary_2<String, GameRequest>();
		profile.OpenedLocations = new List_1();
		profile.Rid = 0;
		profile.Sid = 0;
		profile.SwipeCount = 0;
		profile.Items = new Dictionary_2<String, ItemData>();

		profile.Deck.push(meta.Locations.get("28354825").Over[0].Id);
		LeftRight(meta.Cards.get(profile.Deck[0]).Next, meta, profile, random);
		return profile;
	}

	public static function Change(request:GameRequest, meta:GameMeta, profile:ProfileData, timestamp:Int, response:GameResponse, reward:List_1<RewardMeta>,
			random:Random):Void {
		if (request.Timestamp > timestamp) {
			response.Error = "trigger can be newer then server time";
			return;
		}
		if (request.Rid != profile.Rid) {
			response.Error = "the trigger should have rid " + profile.Rid;
			return;
		}
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
				var location:CardMeta = meta.Locations.get(request.Hash);
				// if (!CheckCondition(location.Act.Con, null, profile)) {
				// 	response.Error = "error loc condition " + request.Value;
				// 	return;
				// }
				profile.CurrentLocation = location.Id;

			case TriggerMeta.SWIPE:
				var deck:List_1<String> = profile.Deck;
				if (deck.getCount() == 0) {
					response.Error = "error deck is empty " + request.Id;
					return;
				}
				if (deck[0] != request.Id) {
					response.Error = "error current card " + request.Id;
					return;
				}
				if ((profile.Left != null || profile.Right != null) && request.Hash == null) {
					response.Error = "hash should not be empty ";
					return;
				}
				if (request.Hash != null && profile.Left == null && profile.Right == null) {
					response.Error = "hash should be empty" + request.Hash;
					return;
				}
				if (request.Hash != profile.Left && request.Hash != profile.Right) {
					response.Error = "hash should match with choice" + request.Hash;
					return;
				}
				random.setStringSeed(Std.string(request.Timestamp));
				deck.pop();

				if (request.Hash != null) {
					var nextCard:CardMeta = meta.Cards.get(request.Hash);
					deck.push(nextCard.Id);
					if (nextCard.Over != null) {
						var over:NativeArray<TriggerMeta> = nextCard.Over;
						var candidates:Array<CardMeta> = GetTempArray();
						for (o in over) {
							var oc:CardMeta = meta.Cards.get(o.Id);
							if (CheckCard(oc, meta, profile, random))
								candidates.push(oc);
						}
						candidates.sort((a, b) -> b.Pri - a.Pri);
						for (c in candidates)
							deck.push(c.Id);
					}
				}
				profile.Left = null;
				profile.Right = null;
				if (deck.getCount() > 0) {
					var nextCard:CardMeta = meta.Cards.get(deck[0]);
					if (nextCard.Next != null) {
						var next:NativeArray<TriggerMeta> = nextCard.Next;
						LeftRight(next, meta, profile, random);
					}
					if (nextCard.Reward != null) {
						for (r in nextCard.Reward) {
							if (!CheckReward(r, meta, profile, random))
								continue;
							var i:ItemData = profile.Items.getOrCreate(r.Id, f -> new ItemData(r.Id, 0));
							i.Count += r.Count;
							reward?.push(r);
						}
					}
				} else {
					profile.Cooldown = timestamp;
				}

				// // Apply card
				var card:CardData = profile.Cards.getOrCreate(request.Id, f -> new CardData(request.Id));
				card.CT++;
				card.Choice = request.Value;
				card.Executed = request.Timestamp;

			case TriggerMeta.REROLL:
				if (profile.Deck.getCount() > 0) {
					response.Error = "cards are available ";
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
				profile.Items.set(id, i);
				profile.Deck.push(meta.Locations.get(profile.CurrentLocation).Over[0].Id);
				profile.Cooldown = 0;
				LeftRight(meta.Cards.get(profile.Deck[0]).Next, meta, profile, random);

			case TriggerMeta.START_GAME:

			case TriggerMeta.EVENT:
				if (request.Hash == null) {
					response.Error = "an event trigger should have a hash";
					return;
				}
				var events:Dictionary_2<String, GameRequest> = profile.Events;
				var r:GameRequest = null;
				events.get(request.Hash);
				if (r == null) {
					response.Error = "profile should have an event with the same hash";
					return;
				}

				// change profile
				var items:Dictionary_2<String, ItemData> = profile.Items;
				var i:ItemData = items.getOrCreate(r.Id, f -> new ItemData(r.Id, 0));
				i.Count += r.Value;
				// items[r.Id] = i;
				events.remove(request.Hash);
			default:
				response.Error = "unexpected trigger ";
				return;
		}
		profile.Rid += 1;
	}

	public static function CheckCard(cardMeta:CardMeta, data:GameMeta, profile:ProfileData, random:Random):Bool {
		if (cardMeta.CT > 0 || cardMeta.CR > 0) {
			var cardData:CardData = profile.Cards.get(cardMeta.Id);
			if (cardData != null && cardData.Executed > 0)
				return false;
		}

		if (cardMeta.Chance > 0 && random.randomInt(0, 100) > cardMeta.Chance)
			return false;

		if (!CheckCondition(cardMeta.Con, data, profile, random))
			return false;

		return true;
	}

	public static function CheckReward(rewardMeta:RewardMeta, data:GameMeta, profile:ProfileData, random:Random):Bool {
		if (rewardMeta.Chance > 0 && random != null && random.randomInt(0, 100) > rewardMeta.Chance)
			return false;

		if (!CheckCondition(rewardMeta.Con, data, profile, random))
			return false;

		return true;
	}

	private static function CheckCondition(con:Null<NativeArray<ConditionMeta>>, data:GameMeta, profile:ProfileData, random:Random):Bool {
		if (con == null || con.length == 0)
			return true;

		for (c in con) {
			switch (c.Type) {
				case ConditionMeta.CARD:
					var card:CardData = profile.Cards.get(c.Id);
					if (card == null || card.CT < c.Count)
						return false;

				case ConditionMeta.ITEM:
					var item:ItemData = profile.Items.get(c.Id);
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

	private static function LeftRight(next:NativeArray<TriggerMeta>, meta:GameMeta, profile:ProfileData, random:Random):Void {
		var candidates:Array<CardMeta> = GetTempArray();
		for (n in next) {
			var nc:CardMeta = meta.Cards.get(n.Id);
			if (CheckCard(nc, meta, profile, random))
				candidates.push(nc);
		}
		if (candidates.length == 0) {
			profile.Left = null;
			profile.Right = null;
			return;
		} else if (candidates.length == 1) {
			profile.Left = candidates[0].Id;
			profile.Right = candidates[0].Id;
			return;
		} else if (candidates.length == 2) {
			profile.Left = candidates[0].Id;
			profile.Right = candidates[1].Id;
			return;
		}
		candidates.sort((a, b) -> b.Pri - a.Pri);
		var first:Int = candidates[0].Pri;
		var filtered:Array<CardMeta> = candidates.filter(c -> c.Pri == first);
		if (filtered.length == 1) {
			profile.Left = filtered[0].Id;
			candidates.remove(filtered[0]);
		} else {
			var d:CardMeta = filtered[random.randomInt(0, filtered.length - 1)];
			profile.Left = d.Id;
			candidates.remove(d);
		}
		candidates.sort((a, b) -> b.Pri - a.Pri); // without left card
		var first:Int = candidates[0].Pri;
		var filtered:Array<CardMeta> = candidates.filter(c -> c.Pri == first);
		if (filtered.length == 1) {
			profile.Right = filtered[0].Id;
		} else {
			profile.Right = filtered[random.randomInt(0, filtered.length - 1)].Id;
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

	@:generic public static function push<T>(_this:cs.system.collections.generic.List_1<T>, x:T):Int {
		_this.Add(x);
		return _this.Count;
	}

	@:generic public static function contains<T>(_this:cs.system.collections.generic.List_1<T>, x:T):Bool {
		return _this.Contains(x);
	}

	public static function pop<T>(_this:cs.system.collections.generic.List_1<T>):T {
		var i:T = _this[0];
		_this.RemoveAt(0);
		return i;
	}

	public static function sort<T>(_this:cs.system.collections.generic.List_1<T>, f:T->T->Int):Void {
		_this.Sort(f);
	}

	@:generic public static function get<K, T>(_this:cs.system.collections.generic.Dictionary_2<K, T>, key:K):T {
		return _this.get_Item(key);
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
	@:generic public static function find<T>(_this:NativeArray<T>, f:T->Bool):Null<T> {
		return Lambda.find(_this, f);
	}

	@:generic public static function getCount<T>(_this:Array<T>):Int {
		return _this.length;
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
