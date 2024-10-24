import haxe.rtti.CType.CTypeTools;
import haxe.rtti.Meta;
import haxe.macro.Expr.Error;
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

	private static var startCard:String = "28549183"; // 27901213"; // "27901222"; // "28387804"; // "28387804"; // "28387804";

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
		return cast Math.floor(cast(timeleft, Float) / (meta.Config.DurationReroll / -meta.Config.PriceReroll[0].Count));
	}

	public static function Left(time:Int64, start:Int64, duration:Int):Int {
		return Int64.toInt((haxe.Int64.ofInt(duration) + start) - time);
	}

	public static function CreateProfile(request:GameRequest, meta:GameMeta, timestamp:Int, response:GameResponse):ProfileData {
		SetRandomSeedByTime(request.Timestamp);

		// there should be getting profile from Mongo database on server side
		var profile:ProfileData = new ProfileData();
		profile.Cards = new Dictionary_2<String, CardData>();
		profile.Cooldown = 0;
		profile.CurrentLocation = "28354825";
		profile.Deck = new List_1();
		profile.Choices = new List_1();
		profile.Accept = new Dictionary_2<String, GameRequest>();
		profile.OpenedLocations = new List_1();
		// profile.CardStates = new List_1<Int>();
		profile.Rid = 0;
		profile.Sid = 0;
		profile.SwipeCount = 0;
		profile.Items = new Dictionary_2<String, ItemData>();
		profile.Items.getOrCreate("6", f -> new ItemData("6", 998));
		//
		// 		profile.Items.getOrCreate("20", f -> new ItemData("20", 998));
		// 		profile.Items.getOrCreate("1", f -> new ItemData("1", 998));
		// 		profile.Items.getOrCreate("64", f -> new ItemData("64", 998));

		// profile.Items.getOrCreate("13", f -> new ItemData("13", 10));
		// profile.Items.getOrCreate("5", f -> new ItemData("5", 2));
		// profile.Items.getOrCreate("11", f -> new ItemData("11", 10));
		// profile.Items.getOrCreate("62", f -> new ItemData("62", 10));

		if (meta.Config.GodMode)
			for (i in 1...64)
				profile.Items.getOrCreate(i + "", f -> new ItemData(i + "", 10));
		// profile.Items.getOrCreate("55", f -> new ItemData("55", 10));
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

		profile.RewardEvents = new Dictionary_2<String, ItemData>();
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
		// profile.Deck.push("27901213");

		setStartCard(profile, meta, random);

		// средний город
		// profile.Deck.push("27905696");
		// profile.CardStates.push(CardData.CHOICE);
		// profile.CardStates.push(CardData.CHOICE);
		// ApplyCardState(meta.Cards.tryGet(GetCurrentCard(profile)), null, meta, profile, random, response);

		return profile;
	}

	private static function SetRandomSeedByTime(timestamp64:Int64):Void {
		random = GetRandomInstance();
		var seed:String = Std.string(timestamp64);
		var newSeed:String = "";
		for (i in 0...5)
			newSeed += seed.charAt(seed.length - i);
		random.setStringSeed(newSeed);
	}

	public static function Change(request:GameRequest, meta:GameMeta, profile:ProfileData, timestamp:Int, response:GameResponse):Void {
		response.Log = null;
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
		profile.RewardEvents = new Dictionary_2<String, ItemData>();
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
				if (profile.Deck.getCount() == 0) {
					response.Error = "error deck is empty " + request.Hash;
					return;
				}
				var currentDeckItem:DeckItem = GetCurrentCard(profile);
				if (currentDeckItem.Id != request.Hash) {
					response.Error = "error current card " + request.Hash;
					return;
				}
				var countChoices:Int = profile.Choices.getCount();
				if (countChoices == 1) {
					response.Error = "choices should be only two or zero";
					return;
				}
				if (request.Id == null && countChoices > 0) {
					response.Error = "id should not be empty ";
					return;
				}
				if (request.Id != null && countChoices == 0) {
					response.Error = "id should be empty" + request.Id;
					return;
				}

				var choiceInfo:ChoiceInfo = request.Id != null ? profile.Choices.findList(function(item) return item.Id == request.Id) : null;
				if (countChoices > 0 && choiceInfo == null) {
					response.Error = "id should match with choice" + request.Id;
					return;
				}

				if (currentDeckItem.State == CardData.DESCRIPTION && currentDeckItem.DescIndex > 0) {
					profile.Deck.pop();
					return;
				}

				SetRandomSeedByTime(request.Timestamp);

				// var swipedCard:CardMeta = meta.Cards.tryGet(request.Hash);
				// if (profile.CardStates.getCount() > 1) {
				// 	profile.CardStates.pop();
				// 	ApplyCardState(swipedCard, nextCardInfo, meta, profile, random, response);
				// 	return;
				// }
				UpdatePlayerData(request, currentDeckItem, choiceInfo, meta, profile, response);
				// profile.CardStates = profile.CardStates.getCount() > 0 ? new List_1<Int>() : profile.CardStates;

				CreateDeck(request.Id, choiceInfo, meta, profile, random, response, requestTimestamp);
			// 				if (nextCardInfo != null && nextCardInfo.Next != null) {
			// 					var nextCard:CardMeta = meta.Cards.tryGet(nextCardInfo.Next);
			// 					RecursiveCreateDeck(nextCard, meta, profile, random, response);
			// 				}
			//
			// 				if ((profile.Left == null && profile.Right == null && swipedCard.Next != null && swipedCard.IfNot != null)
			// 					&& (swipedCard.Call == false || (swipedCard.Call == true && profile.Called != null))) {
			// 					var nextCard:CardMeta = meta.Cards.tryGet(swipedCard.IfNot[0].Id);
			// 					RecursiveCreateDeck(nextCard, meta, profile, random, response);
			// 				}
			//

			// CreateCardStates(currentCard, null, meta, profile, random);
			// ApplyCardState(currentCard, null, meta, profile, random, response);

			// if (currentCard.Call && profile.Called != currentCard.Id)
			// 	profile.Called = currentCard.Id;
			// else if (profile.Called == currentCard.Id)
			// profile.Called = null;

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

				// profile.Deck.push(startCard);
				// profile.Deck.push(

				profile.Cooldown = 0;
				// profile.CardStates = new List_1<Int>();
				profile.Rerolls++;
				profile.RewardEvents.getOrCreate(id, f -> new ItemData(id, 0)).Count += -price;

				for (c in profile.History)
					profile.Cards.tryGet(c).CR = 0;
				profile.History = new List_1();

				setStartCard(profile, meta, random);

				// profile.CardStates.push(CardData.CHOICE);
				var card:CardMeta = meta.Cards.tryGet(profile.Deck[0].Id);

			// ApplyCardState(card, null, meta, profile, random, response);

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

	private static function setStartCard(profile:ProfileData, meta:GameMeta, random:Random) {
		var startCard = meta.Config.StartCard;
		var card = meta.Cards.tryGet(startCard);
		profile.Deck = new List_1();
		RecursiveCreateDeck(card, meta, profile, random, new GameResponse(), 0);
	}

	private static function UpdatePlayerData(request:GameRequest, currentDeckItem:DeckItem, choice:ChoiceInfo, meta:GameMeta, profile:ProfileData,
			response:GameResponse) {
		profile.SwipeCount++;

		// var cardMeta:CardMeta = meta.Cards.tryGet(request.Hash);

		var card:CardData = profile.Cards.getOrCreate(request.Hash, f -> new CardData(request.Hash));
		card.CT++;
		card.CR++;
		response.Debug += "cardID " + card.Id + " CT" + card.CT;

		if (currentDeckItem.State == CardData.CHOICE) {
			// card.Choice = request.Id;
			profile.Choices = new List_1();
		}

		profile.History.push(card.Id);
	}

	private static function CreateDeck(nextCardId:String, choice:ChoiceInfo, meta:GameMeta, profile:ProfileData, random:Random, response:GameResponse,
			requestTimestamp:Int) {
		profile.Deck.pop();

		if (choice != null) {
			if (choice.Id == choice.Next) {
				nextCardId = null;
			} else if (choice.Next != null) {
				RecursiveCreateDeck(meta.Cards.tryGet(choice.Next), meta, profile, random, response, 0);
			}
		}

		var nextCard:CardMeta = null;
		if (nextCardId != null) {
			nextCard = meta.Cards.tryGet(nextCardId);
			RecursiveCreateDeck(nextCard, meta, profile, random, response, 0);
		}

		CheckAndClearDeck(meta, profile, random, response);

		if (profile.Deck.getCount() == 0) {
			profile.Cooldown = requestTimestamp;
			return;
		}

		var deckInfo:DeckItem = GetCurrentCard(profile);

		if (deckInfo.State == CardData.REWARD) {
			nextCard = meta.Cards.tryGet(deckInfo.Id);
			if (nextCard.TradeLimit > 0)
				ApplyTrade(nextCard, choice, meta, profile, random);
			else
				ApplyReward(nextCard, choice, meta, profile, random);
		} else if (deckInfo.State == CardData.CHOICE) {
			nextCard = meta.Cards.tryGet(deckInfo.Id);
			CreateLeftRight(nextCard, meta, profile, random, response);
			if (profile.Choices.getCount() == 0) {
				profile.Deck.removeItem(profile.Deck.findList(d -> d.State == CardData.REWARD && d.Id == deckInfo.Id));
				if (nextCard.IfNot != null) {
					CreateDeck(nextCard.IfNot[0].Id, null, meta, profile, random, response, requestTimestamp);
				} else {
					deckInfo.DescIndex = 0;
					deckInfo.State = CardData.NOTHING;
				}
			} else if (profile.Choices.getCount() == 1) {
				var c = profile.Choices[0];
				profile.Choices = new List_1();
				CreateDeck(c.Id, c, meta, profile, random, response, requestTimestamp);
			} else if (profile.Choices.getCount() > 2)
				throw "profile have more then 2 choices";
		}
	}

	private static function CheckAndClearDeck(meta:GameMeta, profile:ProfileData, random:Random, response:GameResponse) {
		var currentCard:CardMeta = null;
		while (profile.Deck.getCount() > 0) {
			var deckItem = GetCurrentCard(profile);
			currentCard = meta.Cards.tryGet(deckItem.Id);
			if (!CheckCard(currentCard, meta, profile, random, response)) {
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

		if (cardMeta.Chance > 0 && random.randomInt(0, 100) > cardMeta.Chance)
			return false;

		return true;
	}

	private static function RecursiveCreateDeck(nextCard:CardMeta, meta:GameMeta, profile:ProfileData, random:Random, response:GameResponse, deep:Int):Void {
		if (nextCard.Cut) {
			profile.Deck = new List_1();
		}

		// если карта является вызываемой награду выдаем после выбора
		if ((nextCard.Call || nextCard.TradeLimit > 0) && (nextCard.Reward != null || nextCard.Cost != null)) {
			profile.Deck.push(new DeckItem(nextCard.Id, CardData.REWARD, 0));
		}
		// если карта содержит список вызовов поверх
		if (nextCard.Over != null
			&& nextCard.Reward == null
			&& nextCard.Cost == null
			&& (nextCard.RewardText != null || nextCard.IfNothing != null)) {
			var anyOver:Bool = false;
			for (o in nextCard.Over) {
				var card:CardMeta = meta.Cards.tryGet(o.Id);
				if (CheckCard(card, meta, profile, random)) {
					anyOver = true;
					break;
				}
			}
			if (anyOver)
				profile.Deck.push(new DeckItem(nextCard.Id, CardData.REWARD, 0));
			else
				for (i in 0...nextCard.IfNothing.length)
					profile.Deck.push(new DeckItem(nextCard.Id, CardData.NOTHING, i));
		}

		if (nextCard.Next != null || nextCard.TradeLimit > 0) {
			profile.Deck.push(new DeckItem(nextCard.Id, CardData.CHOICE, 0));
		}

		RecursiveCreateCards(nextCard.Over, nextCard.Shake, meta, profile, random, response, deep);

		if (!nextCard.Call && nextCard.TradeLimit == 0 && (nextCard.Reward != null || nextCard.Cost != null)) {
			profile.Deck.push(new DeckItem(nextCard.Id, CardData.REWARD, 0));
		}

		if (nextCard.OnlyOnce != null) {
			var prof = profile.Cards.tryGet(nextCard.Id);
			response.Debug += "ON " + nextCard.Id + " prf" + (prof != null ? prof.CT : -1);
			if (prof == null || prof.CT == 0) {
				for (i in 0...nextCard.OnlyOnce.length)
					profile.Deck.push(new DeckItem(nextCard.Id, CardData.DESCRIPTION, i));
			} else if (nextCard.Descs != null) {
				for (i in 0...nextCard.Descs.length)
					profile.Deck.push(new DeckItem(nextCard.Id, CardData.DESCRIPTION, i));
			}
		} else if (nextCard.Descs != null) {
			for (i in 0...nextCard.Descs.length)
				profile.Deck.push(new DeckItem(nextCard.Id, CardData.DESCRIPTION, i));
		}
	}

	private static function RecursiveCreateCards(triggers:NativeArray<TriggerMeta>, shake:Bool, meta:GameMeta, profile:ProfileData, random:Random,
			response:GameResponse, deep:Int):Void {
		if (triggers == null || triggers.length == 0)
			return;
		var candidates:Array<TriggerMeta> = new Array();
		for (o in triggers) {
			for (i in 0...(o.Count > 0 ? o.Count : 1))
				candidates.push(o);
		}
		if (shake)
			Shake(candidates, random);

		for (o in candidates) {
			var card:CardMeta = meta.Cards.tryGet(o.Id);
			if (CheckCardTrigger(card, o, meta, profile, random)) {
				RecursiveCreateDeck(card, meta, profile, random, response, deep + 1);
				if (o.Over != null) {
					var cardOver:CardMeta = meta.Cards.tryGet(o.Over);
					if (CheckCardTrigger(cardOver, o, meta, profile, random)) {
						RecursiveCreateDeck(cardOver, meta, profile, random, response, deep + 1);
					}
				}
			} else if (o.IfNot != null) {
				var ifnot:CardMeta = meta.Cards.tryGet(o.IfNot);
				if (CheckCardTrigger(ifnot, o, meta, profile, random))
					RecursiveCreateDeck(ifnot, meta, profile, random, response, deep + 1);
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

	public static function CheckCard(card:CardMeta, meta:GameMeta, profile:ProfileData, random:Random, response:GameResponse = null):Bool {
		if (card == null)
			throw "null card ";

		if (card.CT > 0 || card.CR > 0) {
			var cardData:CardData = profile.Cards.tryGet(card.Id);
			if (cardData != null && ((card.CT != 0 && cardData.CT >= card.CT) || (card.CR != 0 && cardData.CR >= card.CR))) {
				return false;
			}
		}

		if (card.TradeLimit > 0) {
			if (card.Cost == null || card.Reward == null)
				throw "card " + card.Id + " must have a cost, because it has a TradeLimit";
			if (!CheckAllCost(card.Cost[0], card, meta, profile, random))
				return false;
			if (!CheckAllReward(card.Reward[0], card, meta, profile, random))
				return false;
		} else if (card.Cost != null && card.Cost.length > 0) {
			var match:Bool = false;
			for (c in card.Cost) {
				if (CheckAllCost(c, card, meta, profile, random)) {
					match = true;
					break;
				}
			}
			if (match == false)
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

	private static function CheckCardsInGroup(group:String, meta:GameMeta, profile:ProfileData, random:Random):Bool {
		var hasNext:Bool = false;
		var cardGroup:GroupMeta = meta.Groups.tryGet(group);
		for (c in cardGroup.Cards) {
			if (c.Type == CardMeta.TYPE_GROUP && CheckCardsInGroup(c.Id, meta, profile, random)) {
				hasNext = true;
				break;
			} else {
				var cm:CardMeta = meta.Cards.tryGet(c.Id);
				if (CheckCard(cm, meta, profile, random)) {
					hasNext = true;
					break;
				}
			}
		}
		return hasNext;
	}

	public static function CheckAllCost(cost:Null<NativeArray<RewardMeta>>, card:CardMeta, meta:GameMeta, profile:ProfileData, random:Random):Bool {
		if (cost == null || cost.length == 0) {
			if (card.TradeLimit > 0)
				throw "card " + card.Id + " must have a cost";
			return true;
		}
		if (card.TradeLimit > 0) {
			var hasTwoCost:Int = 0;
			for (c in cost) {
				if (CheckCost(c, card, meta, profile, random))
					hasTwoCost++;
			}
			if (hasTwoCost < 2)
				return false;
		} else {
			// обычное поведение
			for (c in cost) {
				if (!CheckCost(c, card, meta, profile, random))
					return false;
			}
		}

		return true;
	}

	private static function CheckAllReward(reward:Null<NativeArray<RewardMeta>>, card:CardMeta, meta:GameMeta, profile:ProfileData, random:Random):Bool {
		if (reward == null || reward.length == 0) {
			if (card.TradeLimit > 0)
				throw "card " + card.Id + " must have a reward";
			return true;
		}
		var hasReward:Int = 0;
		for (r in reward) {
			if (GetRewardMinCount(r.Id, card, meta) > 0)
				hasReward++;
		}
		if (hasReward < 1)
			return false;

		return true;
	}

	public static function GetRewardMinCount(id:String, card:CardMeta, meta:GameMeta):Int {
		return Math.ceil(card.TradeLimit / meta.Items.tryGet(id).Price);
	}

	public static function CheckCost(cost:RewardMeta, card:CardMeta, meta:GameMeta, profile:ProfileData, random:Random):Bool {
		var item:ItemData = profile.Items.tryGet(cost.Id);
		var count:Int = item != null ? item.Count : 0;
		if (card.TradeLimit > 0) {
			var minCount = GetRewardMinCount(cost.Id, card, meta);
			return minCount > 0 && count >= minCount;
		}
		return count >= cost.Count;
	}

	public static function CheckCondition(con:Null<NativeArray<ConditionMeta>>, meta:GameMeta, profile:ProfileData, random:Random):Bool {
		if (con == null || con.length == 0)
			return true;

		for (c in con) {
			switch (c.Type) {
				case ConditionMeta.CARD:
					if (c.Sign == "A") {
						var cm:CardMeta = meta.Cards.tryGet(c.Id);
						var invert:Bool = c.Count == 1;
						if (invert && IfCardLeftRightAvailable(cm.Next, cm, meta, profile, random))
							return false;
						if (!invert && !IfCardLeftRightAvailable(cm.Next, cm, meta, profile, random))
							return false;
						continue;
					}

					if (c.Sign == "B") {
						var cm:CardMeta = meta.Cards.tryGet(c.Id);
						var invert:Bool = c.Count == 1;
						if (invert && CheckCard(cm, meta, profile, random))
							return false;
						if (!invert && !CheckCard(cm, meta, profile, random))
							return false;
						continue;
					}

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

	private static function GetRewardIndex(card:CardMeta, meta:GameMeta, profile:ProfileData, random:Random):Int {
		var conds:NativeArray<NativeArray<ConditionMeta>> = card.Con;
		if (card.Reward != null)
			for (index in 0...card.Reward.length) {
				var cond:NativeArray<ConditionMeta> = conds == null ? null : (index >= conds.length ? null : conds[index]);
				if (CheckCondition(cond, meta, profile, random)) {
					return index;
				}
			}
		return -1;
	}

	private static function GetCostIndex(card:CardMeta, meta:GameMeta, profile:ProfileData, random:Random):Int {
		var conds:NativeArray<NativeArray<ConditionMeta>> = card.Con;
		var costs:NativeArray<NativeArray<RewardMeta>> = card.Cost;
		if (costs != null)
			for (index in 0...costs.length) {
				var cond:NativeArray<ConditionMeta> = conds == null ? null : (index >= conds.length ? null : conds[index]);
				var cost:NativeArray<RewardMeta> = costs == null ? null : (index >= costs.length ? null : costs[index]);
				if (CheckCondition(cond, meta, profile, random) && CheckAllCost(cost, card, meta, profile, random)) {
					return index;
				}
			}
		return -1;
	}

	private static function ApplyReward(card:CardMeta, info:ChoiceInfo, meta:GameMeta, profile:ProfileData, random:Random):Void {
		var rewardIndex = info != null ? info.RewardIndex : GetRewardIndex(card, meta, profile, random);
		var costIndex = info != null ? info.CostIndex : GetCostIndex(card, meta, profile, random);
		if (rewardIndex != -1)
			for (r in card.Reward[rewardIndex]) {
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
				profile.RewardEvents.getOrCreate(r.Id, f -> new ItemData(r.Id, 0)).Count += r.Count;
			}

		if (costIndex != -1)
			for (c in card.Cost[costIndex]) {
				if (!CheckReward(c, meta, profile, random))
					continue;
				switch (c.Type) {
					case ConditionMeta.ITEM:
						var i:ItemData = profile.Items.getOrCreate(c.Id, f -> new ItemData(c.Id, 0));
						i.Count -= c.Count;
						i.Count = i.Count < 0 ? 0 : i.Count;
				}
				profile.RewardEvents.getOrCreate(c.Id, f -> new ItemData(c.Id, 0)).Count -= c.Count;
			}
	}

	public static function GetCurrentCard(profile:ProfileData):DeckItem {
		return profile.Deck[profile.Deck.getCount() - 1];
	}

	private static function RecursiveGroupLeftRight(candidates:Array<CardMeta>, nextDict:Dictionary_2<String, String>, nextCard:String,
			next:NativeArray<TriggerMeta>, meta:GameMeta, profile:ProfileData, random:Random):Void {
		for (n in next) {
			if (n.Type == CardMeta.TYPE_GROUP) {
				var group:GroupMeta = meta.Groups.tryGet(n.Id);
				RecursiveGroupLeftRight(candidates, nextDict, nextCard, group.Cards, meta, profile, random);
			} else {
				var nc:CardMeta = meta.Cards.tryGet(n.Id);
				if (CheckCardTrigger(nc, null, meta, profile, random) && CheckCard(nc, meta, profile, random)) {
					candidates.push(nc);
					nextDict.set(nc.Id, nextCard);
				}
			}
		}
	}

	private static function IfCardLeftRightAvailable(next:NativeArray<TriggerMeta>, current:CardMeta, meta:GameMeta, profile:ProfileData, random:Random):Bool {
		if (next == null)
			return false;

		var candidates:Array<CardMeta> = new Array<CardMeta>();
		var nextDict:Dictionary_2<String, String> = new Dictionary_2<String, String>();

		for (n in next) {
			if (n.Type == CardMeta.TYPE_GROUP) {
				var group:GroupMeta = meta.Groups.tryGet(n.Id);
				RecursiveGroupLeftRight(candidates, nextDict, n.Next, group.Cards, meta, profile, random);
			} else {
				var nc:CardMeta = meta.Cards.tryGet(n.Id);
				if (CheckCardTrigger(nc, null, meta, profile, random) && CheckCard(nc, meta, profile, random)) {
					candidates.push(nc);
				}
			}
		}
		return candidates.length > 0;
	}

	private static function CreateLeftRight(current:CardMeta, meta:GameMeta, profile:ProfileData, random:Random, response:GameResponse):Void {
		var candidates:Array<Array<CardMeta>> = new Array<Array<CardMeta>>();
		candidates.push(new Array<CardMeta>());
		var groups:Int = 0;
		var nextDict:Dictionary_2<String, String> = new Dictionary_2<String, String>();

		// если это карта торговли
		if (current.TradeLimit > 0) {
			var available:Array<RewardMeta> = new Array<RewardMeta>();
			var reward:Array<RewardMeta> = new Array<RewardMeta>();

			for (c in current.Cost[0])
				if (CheckCost(c, current, meta, profile, random))
					available.push(c);

			for (r in current.Reward[0])
				if (GetRewardMinCount(r.Id, current, meta) > 0)
					reward.push(r);

			if (available.length == 0 || reward.length == 0)
				return;

			Shake(available, random);

			for (a in available) {
				Shake(reward, random);
				var rewardIndex:Int = 0;
				while (rewardIndex < reward.length) {
					var r:RewardMeta = reward[rewardIndex];
					if (r.Id != a.Id) {
						var choice:ChoiceInfo = new ChoiceInfo(current.Id, current.Id, current.Reward[0].IndexOf(r), current.Id, current.Cost[0].IndexOf(a));
						profile.Choices.push(choice);
						break;
					}
					rewardIndex++;
				}
				if (profile.Choices.getCount() >= 2)
					break;
			}
			return;
		}
		if (current.Next != null)
			for (n in current.Next) {
				if (n.Type == CardMeta.TYPE_GROUP) {
					var group:GroupMeta = meta.Groups.tryGet(n.Id);
					candidates.push(new Array<CardMeta>());
					groups++;
					RecursiveGroupLeftRight(candidates[groups], nextDict, n.Next, group.Cards, meta, profile, random);
				} else {
					var nc:CardMeta = meta.Cards.tryGet(n.Id);
					if (CheckCardTrigger(nc, null, meta, profile, random) && CheckCard(nc, meta, profile, random)) {
						candidates[0].push(nc);
						nextDict.set(nc.Id, n.Next);
					}
				}
			}
		if (current.Shure != null && current.Next != null) {
			candidates[0].push(current);
			nextDict.set(current.Id, current.Id);
		}
		profile.Choices = new List_1();

		response.Debug += "c" + candidates.length + " j" + Json.stringify(candidates);
		response.Debug += " NEXT" + Json.stringify(nextDict);

		if (candidates.length == 1 && candidates[0].length == 0) {
			return;
		} else if (candidates.length == 1 && candidates[0].length == 1) {
			profile.Choices.push(new ChoiceInfo(candidates[0][0].Id, nextDict.tryGet(candidates[0][0].Id),
				GetRewardIndex(candidates[0][0], meta, profile, random), current.Id, GetCostIndex(candidates[0][0], meta, profile, random)));
			return;
		} else if (candidates.length == 1 && candidates[0].length == 2) {
			if (current.Shake == true)
				Shake(candidates[0], random);
			profile.Choices.push(new ChoiceInfo(candidates[0][0].Id, nextDict.tryGet(candidates[0][0].Id),
				GetRewardIndex(candidates[0][0], meta, profile, random), current.Id, GetCostIndex(candidates[0][0], meta, profile, random)));
			if (!current.OneNext)
				profile.Choices.push(new ChoiceInfo(candidates[0][1].Id, nextDict.tryGet(candidates[0][1].Id),
					GetRewardIndex(candidates[0][1], meta, profile, random), current.Id, GetCostIndex(candidates[0][0], meta, profile, random)));
			return;
		}
		for (c in candidates.copy()) {
			if (c.length == 0) {
				candidates.remove(c);
				continue;
			}
		}
		for (c in candidates) {
			Shake(c, random);
			c.sort((a, b) -> b.Pri - a.Pri);
		}
		Shake(candidates, random);
		var left = candidates.length > 0 ? candidates[0][0] : null;
		var right = candidates.length > 1 ? candidates[1][0] : null;
		if (right == null)
			right = candidates.length > 0 && candidates[0].length > 1 ? candidates[0][1] : null;

		if (left != null)
			profile.Choices.push(new ChoiceInfo(left.Id, nextDict.tryGet(left.Id), GetRewardIndex(left, meta, profile, random), current.Id,
				GetCostIndex(left, meta, profile, random)));
		if (right != null && !current.OneNext)
			profile.Choices.push(new ChoiceInfo(right.Id, nextDict.tryGet(right.Id), GetRewardIndex(right, meta, profile, random), current.Id,
				GetCostIndex(left, meta, profile, random)));
	}

	private static function Shake<T>(arr:Array<T>, random:Random):Void {
		var n:Int = arr.length;
		for (j in 0...2)
			for (i in 0...n) {
				var randomIndex:Int = random.randomInt(0, n - 1);
				var temp:T = arr[i];
				arr[i] = arr[randomIndex];
				arr[randomIndex] = temp;
			}
	}

	public static function HasVisibleReward(currentCard:CardMeta, meta:GameMeta):Bool {
		if (currentCard.Reward != null)
			for (rr in currentCard.Reward)
				for (r in rr)
					if (!meta.Items.tryGet(r.Id).Hidden)
						return true;
		if (currentCard.Cost != null)
			for (rr in currentCard.Cost)
				for (r in rr)
					if (!meta.Items.tryGet(r.Id).Hidden)
						return true;
		return false;
	}

	private static function ApplyTrade(card:CardMeta, choice:ChoiceInfo, meta:GameMeta, profile:ProfileData, random:Random):Void {
		var r:RewardMeta = card.Reward[0][choice.RewardIndex];
		if (CheckReward(r, meta, profile, random)) {
			switch (r.Type) {
				case ConditionMeta.ITEM:
					var i:ItemData = profile.Items.getOrCreate(r.Id, f -> new ItemData(r.Id, 0));
					var ct:Int = GetRewardMinCount(r.Id, card, meta);
					i.Count += ct;
					i.Count = i.Count < 0 ? 0 : i.Count;
					profile.RewardEvents.getOrCreate(r.Id, f -> new ItemData(r.Id, 0)).Count += ct;
			}
		}
		var c:RewardMeta = card.Cost[0][choice.CostIndex];
		if (CheckReward(c, meta, profile, random)) {
			switch (c.Type) {
				case ConditionMeta.ITEM:
					var i:ItemData = profile.Items.getOrCreate(c.Id, f -> new ItemData(c.Id, 0));
					var ct:Int = GetRewardMinCount(c.Id, card, meta);
					i.Count -= ct;
					i.Count = i.Count < 0 ? 0 : i.Count;
					profile.RewardEvents.getOrCreate(c.Id, f -> new ItemData(c.Id, 0)).Count -= ct;
			}
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

	@:generic public static function IndexOf<T>(_this:NativeArray<T>, e:T):Int {
		var index:Int = 0;
		for (c in _this) {
			if (c == e)
				return index;
			index++;
		}
		return -1;
	}

	public static function iterator<T>(hashSet:cs.system.collections.generic.IEnumerable_1<T>):Iterator<T> {
		return new IEnumerableIterator(hashSet);
	}

	@:generic public static function findList<T>(_this:cs.system.collections.generic.List_1<T>, f:T->Bool):Null<T> {
		for (c in _this) {
			if (f(c))
				return c;
		}
		return null;
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
	@:generic public static function IndexOf<T>(_this:NativeArray<T>, e:T):Int {
		return Lambda.indexOf(_this, e);
	}

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

	@:generic public static function findList<T>(_this:NativeArray<T>, f:T->Bool):Null<T> {
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
