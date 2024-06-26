// Profile Data
import haxe.Int64;
import GameMeta;
import SL;
#if cs
import cs.system.collections.generic.Dictionary_2;
import cs.system.collections.generic.List_1;
import cs.system.SerializableAttribute;
import cs.system.Attribute;
#else
#end
@:nativeGen
@:strict(SerializableAttribute)
class ProfileData {
	public function new() {}

	public var SwipeCount:Int;
	public var Deck:List_1<String>;
	public var Left:CardNextInfo;
	public var Right:CardNextInfo;
	public var CardStates:List_1<Int>;
	public var Called:String;

	public var Cards:Dictionary_2<String, CardData>;
	public var Items:Dictionary_2<String, ItemData>;
	public var Skills:List_1<String>;
	public var Accept:Dictionary_2<String, GameRequest>;
	public var Triggers:Dictionary_2<String, ItemTypeData>;

	public var ActiveQuests:List_1<String>;

	// public List<SkillVO> Skills;
	public var Sid:Int;
	public var Rid:Int;
	// public var SwipeReroll:Int;
	public var Cooldown:Int;

	public var OpenedLocations:List_1<String>;
	public var CurrentLocation:String;
	public var LastChange:Int;
	public var Created:Int;
	public var Rerolls:Int;
	public var History:List_1<String>;

	// EVENTS
	public var RewardEvents:Dictionary_2<String, ItemData>;
	public var QuestEvent:String;
}

@:nativeGen
@:strict(SerializableAttribute)
class CardData {
	public static inline var DESCRIPTION:Int = 0;
	public static inline var REWARD:Int = 1;
	public static inline var CHOICE:Int = 2;
	public static inline var NOTHING:Int = 3;
	public static inline var ONLY_ONCE:Int = 4;

	public function new(Id:String) {
		this.Id = Id;
		this.CT = 0;
		this.CR = 0;
		this.Choice = null;
		this.Value = 0;
	}

	public var CR:Int;
	public var Id:String;
	public var CT:Int;
	public var Choice:String;
	public var Value:Int;
}

@:nativeGen
@:strict(SerializableAttribute)
class ItemData {
	public function new(Id:String, Count:Int) {
		this.Id = Id;
		this.Count = Count;
	}

	public function Copy():ItemData {
		return new ItemData(this.Id, this.Count);
	}

	public var Id:String;
	public var Count:Int;
}

@:nativeGen
@:strict(SerializableAttribute)
class ItemTypeData extends ItemData {
	public function new(Id:String, Count:Int, Type:Int) {
		super(Id, Count);
		this.Type = Type;
	}

	public var Type:Int;
}

@:nativeGen
@:strict(SerializableAttribute)
class GameRequest {
	public function new(Type:Int, Value:Int = 0, Id:String = "") {
		this.Id = Id;
		this.Type = Type;
		this.Value = Value;
	}

	public var Rid:Int;
	public var Timestamp:Int64;
	public var Id:String;
	public var Type:Int;
	public var Tags:List_1<String>;
	public var Value:Int;
	public var Hash:String;
	public var Version:Int;
}

@:nativeGen
@:strict(SerializableAttribute)
class GameResponse {
	public var Error:String;
	public var Profile:ProfileData;
	public var Events:List_1<GameRequest>;

	//
	public var Debug:String;
	public var Log:String;

	public var Timestamp:Int;
}

@:nativeGen
@:strict(SerializableAttribute)
class CardNextInfo {
	public function new(Id:String, Next:String, RewardIndex:Int) {
		this.Id = Id;
		this.Next = Next;
		this.RewardIndex = RewardIndex;
	}

	public var RewardIndex:Int;
	public var Id:String;
	public var Next:String;
}

// class CardMetaNextInfo {
// 	public var Card:CardMeta;
// 	public var Next:String;
// }
