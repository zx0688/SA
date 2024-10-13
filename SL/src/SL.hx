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
		return cast Math.ceil(cast(timeleft, Float) / (meta.Config.DurationReroll / -meta.Config.PriceReroll[0].Count));
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
		RecursiveCreateDeck(card, meta, profile, random, null, 0);
	}

	private static function UpdatePlayerData(request:GameRequest, currentDeckItem:DeckItem, choice:ChoiceInfo, meta:GameMeta, profile:ProfileData,
			response:GameResponse) {
		profile.SwipeCount++;

		var cardMeta:CardMeta = meta.Cards.tryGet(request.Hash);

		response.Debug += " d" + currentDeckItem.State + " id" + currentDeckItem.Id;
		var card:CardData = profile.Cards.getOrCreate(request.Hash, f -> new CardData(cardMeta.Id));
		if ((cardMeta.Next != null && currentDeckItem.State == CardData.CHOICE)
			|| (cardMeta.Next == null && (cardMeta.Reward != null || cardMeta.Cost != null) && currentDeckItem.State == CardData.REWARD)
			|| (cardMeta.Next == null && cardMeta.Reward == null && cardMeta.Cost == null)) {
			card.CT++;
			card.CR++;
		}

		response.Debug += " ct" + card.CT;

		if (currentDeckItem.State == CardData.CHOICE) {
			// card.Choice = request.Id;
			profile.Choices = new List_1();
		}

		profile.History.push(cardMeta.Id);
	}

	private static function CreateDeck(nextCardId:String, choice:ChoiceInfo, meta:GameMeta, profile:ProfileData, random:Random, response:GameResponse,
			requestTimestamp:Int) {
		profile.Deck.pop();

		if (choice != null) {
			if (choice.Id != null) {
				var card = meta.Cards.tryGet(choice.Id);
				// убираем награду базы если карта блокирует награду
				if (card.BlockReward && choice.Base != null) {
					profile.Deck.removeItem(profile.Deck.findList(d -> d.State == CardData.REWARD && d.Id == choice.Base));
				}
			}

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

		CheckAndClearDeck(meta, profile, random);

		if (profile.Deck.getCount() == 0) {
			profile.Cooldown = requestTimestamp;
			return;
		}

		var deckInfo:DeckItem = GetCurrentCard(profile);

		if (deckInfo.State == CardData.REWARD) {
			nextCard = meta.Cards.tryGet(deckInfo.Id);
			ApplyReward(nextCard, GetRewardIndex(nextCard, meta, profile, random), meta, profile, random);
		} else if (deckInfo.State == CardData.CHOICE) {
			nextCard = meta.Cards.tryGet(deckInfo.Id);
			response.Debug = "Choice: " + nextCard.Name + " (" + nextCard.Id + ")";
			CreateLeftRight(nextCard.Next, nextCard, meta, profile, random, response);
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

	// private static function CreateCardStates(currentCard:CardMeta, choice:ChoiceInfo, meta:GameMeta, profile:ProfileData, random:Random) {
	// 	// only show choice and only text
	// 	if (profile.Called == currentCard.Id) {
	// 		if (HasVisibleReward(currentCard, meta)) {} // profile.CardStates.push(CardData.REWARD);
	// 		else {
	// 			var index = choice != null ? choice.RewardIndex : GetRewardIndex(currentCard, meta, profile, random);
	// 			ApplyReward(currentCard, index, meta, profile, random);
	// 		}
	// 		return;
	// 	}
	// if (currentCard.Next != null) {ç
	// 	if (currentCard.IfNothing != null && !IfCardLeftRightAvailable(currentCard.Next, currentCard, meta, profile, random)) {
	// 		for (i in 0...currentCard.IfNothing.length)
	// 			profile.CardStates.push(CardData.NOTHING);
	// 	} else
	// 		profile.CardStates.push(CardData.CHOICE);
	// }
	// if (currentCard.Call == false) {
	// 	// only show choice and only text
	// 	if (HasVisibleReward(currentCard, meta))
	// 		profile.CardStates.push(CardData.REWARD);
	// 	else {
	// 		var index = choice != null ? choice.RewardIndex : GetRewardIndex(currentCard, meta, profile, random);
	// 		ApplyReward(currentCard, index, meta, profile, random);
	// 	}
	// }
	// 		var showOnlyOnce:Bool = false;
	// 		if (currentCard.OnlyOnce != null && currentCard.OnlyOnce.length > 0) {
	// 			var cardProfile:CardData = profile.Cards.tryGet(currentCard.Id);
	// 			if (cardProfile == null || cardProfile.CT == 0) {
	// 				for (i in 0...currentCard.OnlyOnce.length)
	// 					profile.CardStates.push(CardData.ONLY_ONCE);
	// 				showOnlyOnce = true;
	// 			}
	// 		}
	//
	// 		if (currentCard.Descs != null && currentCard.Descs.length > 0 && showOnlyOnce == false) {
	// 			if (currentCard.RStory == true) {
	// 				profile.CardStates.push(CardData.DESCRIPTION);
	// 			} else
	// 				for (i in 0...currentCard.Descs.length)
	// 					profile.CardStates.push(CardData.DESCRIPTION);
	// 		}
	// }
	// private static function ApplyCardState(currentCard:CardMeta, choice:ChoiceInfo, meta:GameMeta, profile:ProfileData, random:Random, response:GameResponse) {
	// 	switch (GetCurrentState(profile)) {
	// 		case CardData.CHOICE:
	// 			CreateLeftRight(currentCard.Next, currentCard, meta, profile, random, response);
	// 		case CardData.REWARD:
	// 			var index:Int = choice != null ? choice.RewardIndex : GetRewardIndex(currentCard, meta, profile, random);
	// 			ApplyReward(currentCard, index, meta, profile, random);
	// 		case CardData.DESCRIPTION:
	// 			// nothing to do
	// 	}
	// }

	private static function CheckAndClearDeck(meta:GameMeta, profile:ProfileData, random:Random) {
		var currentCard:CardMeta = null;
		while (profile.Deck.getCount() > 0) {
			var deckItem = GetCurrentCard(profile);
			currentCard = meta.Cards.tryGet(deckItem.Id);
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

		if (cardMeta.Chance > 0 && random.randomInt(0, 100) > cardMeta.Chance)
			return false;

		return true;
	}

	private static function RecursiveCreateDeck(nextCard:CardMeta, meta:GameMeta, profile:ProfileData, random:Random, response:GameResponse, deep:Int):Void {
		if (nextCard.Cut) {
			profile.Deck = new List_1();
		}

		// если карта является вызываемой награду выдаем после выбора
		if (nextCard.Call && (nextCard.Reward != null || nextCard.Cost != null)) {
			profile.Deck.push(new DeckItem(nextCard.Id, CardData.REWARD, 0));
		}

		if (nextCard.Next != null) {
			profile.Deck.push(new DeckItem(nextCard.Id, CardData.CHOICE, 0));
		}
		// RecursiveCreateCards(currentCard.Under, currentCard.Shake, meta, profile, random, response);

		RecursiveCreateCards(nextCard.Over, nextCard.Shake, meta, profile, random, response, deep);

		if (!nextCard.Call && (nextCard.Reward != null || nextCard.Cost != null)) {
			profile.Deck.push(new DeckItem(nextCard.Id, CardData.REWARD, 0));
		}

		if (nextCard.OnlyOnce != null) {
			var prof = profile.Cards.tryGet(nextCard.Id);
			if (prof == null || prof.CT == 0)
				for (i in 0...nextCard.OnlyOnce.length)
					profile.Deck.push(new DeckItem(nextCard.Id, CardData.DESCRIPTION, i));
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

	public static function CheckCard(card:CardMeta, meta:GameMeta, profile:ProfileData, random:Random):Bool {
		if (card == null)
			throw "null card ";

		if (card.CT > 0 || card.CR > 0) {
			var cardData:CardData = profile.Cards.tryGet(card.Id);
			if (cardData != null && ((card.CT != 0 && cardData.CT >= card.CT) || (card.CR != 0 && cardData.CR >= card.CR))) {
				return false;
			}
		}

		if (card.Cost != null && card.Cost.length > 0) {
			var match:Bool = false;
			for (c in card.Cost) {
				if (CheckCost(c, meta, profile, random)) {
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

	public static function CheckCost(cost:Null<NativeArray<RewardMeta>>, meta:GameMeta, profile:ProfileData, random:Random):Bool {
		if (cost == null || cost.length == 0)
			return true;

		for (c in cost) {
			var item:ItemData = profile.Items.tryGet(c.Id);
			var count:Int = item != null ? item.Count : 0;
			if (!(count >= c.Count))
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
		var costs:NativeArray<NativeArray<RewardMeta>> = card.Cost;

		if (card.Reward != null)
			for (index in 0...card.Reward.length) {
				var cond:NativeArray<ConditionMeta> = conds == null ? null : (index >= conds.length ? null : conds[index]);
				var cost:NativeArray<RewardMeta> = costs == null ? null : (index >= costs.length ? null : costs[index]);
				if (CheckCondition(cond, meta, profile, random) && CheckCost(cost, meta, profile, random)) {
					return index;
				}
			}
		if (costs != null)
			for (index in 0...costs.length) {
				var cond:NativeArray<ConditionMeta> = conds == null ? null : (index >= conds.length ? null : conds[index]);
				var cost:NativeArray<RewardMeta> = costs == null ? null : (index >= costs.length ? null : costs[index]);
				if (CheckCondition(cond, meta, profile, random) && CheckCost(cost, meta, profile, random)) {
					return index;
				}
			}
		return -1;
	}

	private static function ApplyReward(card:CardMeta, index:Int, meta:GameMeta, profile:ProfileData, random:Random):Void {
		if (index == -1)
			return;

		if (card.Reward != null)
			for (r in card.Reward[index]) {
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

		if (card.Cost != null)
			for (c in card.Cost[index]) {
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

	private static function IfCardLeftRightAvailable(next:NativeArray<TriggerMeta>, current:CardMeta, meta:GameMeta, profile:ProfileData, random:Random):Bool {
		if (next == null)
			return false;

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
				}
			}
		}
		return candidates.length > 0;
	}

	private static function CreateLeftRight(next:NativeArray<TriggerMeta>, current:CardMeta, meta:GameMeta, profile:ProfileData, random:Random,
			response:GameResponse):Void {
		var candidates:Array<Array<CardMeta>> = new Array<Array<CardMeta>>();
		candidates.push(new Array<CardMeta>());
		var groups:Int = 0;
		var nextDict:Dictionary_2<String, String> = new Dictionary_2<String, String>();

		for (n in next) {
			if (n.Type == CardMeta.TYPE_GROUP) {
				var group:GroupMeta = meta.Groups.tryGet(n.Id);
				candidates.push(new Array<CardMeta>());
				groups++;
				RecursiveGroup(candidates[groups], nextDict, n.Next, group.Cards, meta, profile, random);
			} else {
				var nc:CardMeta = meta.Cards.tryGet(n.Id);
				if (CheckCard(nc, meta, profile, random)) {
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
		if (candidates.length == 1 && candidates[0].length == 0) {
			return;
		} else if (candidates.length == 1 && candidates[0].length == 1) {
			profile.Choices.push(new ChoiceInfo(candidates[0][0].Id, nextDict.tryGet(candidates[0][0].Id),
				GetRewardIndex(candidates[0][0], meta, profile, random), current.Id));
			return;
		} else if (candidates.length == 1 && candidates[0].length == 2) {
			if (current.Shake == true)
				Shake(candidates[0], random);
			profile.Choices.push(new ChoiceInfo(candidates[0][0].Id, nextDict.tryGet(candidates[0][0].Id),
				GetRewardIndex(candidates[0][0], meta, profile, random), current.Id));
			profile.Choices.push(new ChoiceInfo(candidates[0][1].Id, nextDict.tryGet(candidates[0][1].Id),
				GetRewardIndex(candidates[0][1], meta, profile, random), current.Id));
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
		// looking for right card
		// if (right == null)
		// 	for (i in 1...candidates.length + 1) {
		// 		if (i == candidates.length) {
		// 			right = candidates[0][1];
		// 		} else if (candidates[candidates.length - i][0] != left) {
		// 			right = candidates[candidates.length - i][0];
		// 			break;
		// 		}
		// 	}
		if (left != null)
			profile.Choices.push(new ChoiceInfo(left.Id, nextDict.tryGet(left.Id), GetRewardIndex(left, meta, profile, random), current.Id));

		if (right != null)
			profile.Choices.push(new ChoiceInfo(right.Id, nextDict.tryGet(right.Id), GetRewardIndex(right, meta, profile, random), current.Id));
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
