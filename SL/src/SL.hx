import GameData;
import haxe.macro.Context;

class SL {
	// private static var random:Random;
	static function main() {
		//	var random = new Random();

		// Seedy.randomInt(0, 10);
	}

	public static function GetNewRandom(timestamp:Int, min:Int, max:Int):Int {
		return 4;
	}

	public static function Change(tr:GameTriggerData, profile:Dynamic, data:Dynamic, timestamp:Int, changes:Dynamic->Void, response:Dynamic):Void {
		if (tr.timestamp > timestamp) {
			response.error = "the trigger should have correct timestamp";
			return;
		}

		var rid:Int = cast Reflect.getProperty(profile, "rid");
		if (tr.rid != rid) {
			response.error = "the trigger should have rid " + rid;
			return;
		}

		// random.setStringSeed(Std.string(tr.timestamp));
		// var i = random.randomInt(0, 100);
		switch (tr.type) {
			case "swipe":
				var deck:List<Int> = cast Reflect.getProperty(profile, "deck");
				if (!Lambda.exists(deck, item -> item == tr.id)) {
					response.error = "profile's deck should contain card " + tr.id;
					return;
				}

				var cards:Map<Int, Dynamic> = cast Reflect.getProperty(data, "cards");
				var cardData:Dynamic = cards.get(tr.id);

				// change profile
				var executed:Map<Int, Int> = cast Reflect.getProperty(profile, "executed");
				executed.set(tr.id, tr.value);
				deck.remove(tr.id);

			case "event":
				if (tr.hash == null) {
					response.error = "an event trigger should have a hash";
					return;
				}
				var events:Map<String, GameTriggerData> = cast Reflect.getProperty(profile, "events");
				var d:GameTriggerData = events.get(tr.hash);
				if (d == null) {
					response.error = "profile should have an event with the same hash";
					return;
				}

				// change profile
				var items:Map<Int, Int> = cast Reflect.getProperty(profile, "items");
				var i:Int = items.get(d.id) ?? 0;
				items.set(d.id, i + d.value);
				events.remove(tr.hash);
		}

		Reflect.setProperty(profile, "rid", tr.rid + 1);
	}

	public static function CheckCondition(tr:GameTriggerData, profile:Dynamic, data:Dynamic, timestamp:Int, changes:Dynamic->Void, response:Dynamic):Void {
		Reflect.setProperty(profile, "rid", tr.rid + 1);
	}
}
