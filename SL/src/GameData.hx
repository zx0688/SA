// Profile Data
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
	public var Left:String;
	public var Right:String;

	public var Cards:Dictionary_2<String, CardData>;
	public var Items:Dictionary_2<String, ItemData>;
	public var Accept:Dictionary_2<String, GameRequest>;

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

	// TEMP
	public var RewardEvent:List_1<RewardData>;
	public var QuestEvent:String;
}

@:nativeGen
@:strict(SerializableAttribute)
class CardData {
	public function new(Id:String) {
		this.Id = Id;
		this.CT = 0;
		this.CR = 0;
		this.Choice = null;
		this.Value = 0;
	}

	public var Id:String;
	public var CR:Int;
	public var CT:Int;
	public var Executed:Int;
	public var Choice:String;
	public var Value:Int;
}

@:nativeGen
@:strict(SerializableAttribute)
class RewardData {
	public function new(Id:String, Count:Int) {
		this.Id = Id;
		this.Count = Count;
	}

	public var Id:String;
	public var Count:Int;
}

@:nativeGen
@:strict(SerializableAttribute)
class ItemData {
	public function new(Id:String, Count:Int) {
		this.Id = Id;
		this.Count = Count;
	}

	public var Id:String;
	public var Count:Int;
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
	public var Timestamp:Int;
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

	public var Timestamp:Int;
}
