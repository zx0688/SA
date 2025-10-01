// GameMeta
import SL;
#if cs
import cs.NativeArray;
import cs.system.collections.generic.List_1;
import cs.system.collections.generic.Dictionary_2;
import cs.system.SerializableAttribute;
import cs.system.Attribute;
#else
#end
@:nativeGen
@:strict(SerializableAttribute)
class GameMeta {
	public var Cards:Dictionary_2<String, CardMeta>;
	// public var All:NativeArray<CardMeta>;
	public var Items:Dictionary_2<String, ItemMeta>;
	public var Heroes:Dictionary_2<String, HeroMeta>;
	public var Skills:Dictionary_2<String, SkillMeta>;
	public var Locations:Dictionary_2<String, CardMeta>;
	public var Groups:Dictionary_2<String, GroupMeta>;
	public var Triggers:Dictionary_2<String, TriggerActionMeta>;

	public var Profile:PlayerMeta;
	public var Config:ConfigMeta;
	public var Version:Int;
}

@:nativeGen
@:strict(SerializableAttribute)
class PlayerMeta {
	public var Cards:NativeArray<Int>;
	public var Reward:NativeArray<NativeArray<RewardMeta>>;
	public var Tags:NativeArray<String>;
	public var Locations:String;
}

@:nativeGen
@:strict(SerializableAttribute)
class ConfigMeta {
	public var DurationReroll:Int;
	public var PriceReroll:NativeArray<RewardMeta>;
	public var StartCard:String;
	public var GodMode:Bool;
	public var TutorialCard:String;
	public var DisableTutorial:Bool;
}

@:nativeGen
@:strict(SerializableAttribute)
class RewardMeta {
	public var Id:String;
	public var Type:Int;
	public var Tags:NativeArray<String>;
	public var Chance:Float;
	public var Count:Int;
	public var Random:NativeArray<Int>;
	public var Con:NativeArray<ConditionMeta>;
}

@:nativeGen
@:strict(SerializableAttribute)
class SkillMeta extends ItemMeta {
	public var Slot:Int;
	public var Values:NativeArray<Int>;
	public var Descs:NativeArray<String>;
	public var Icon:String;
}

@:nativeGen
@:strict(SerializableAttribute)
class HeroMeta extends ItemMeta {
	public var Cards:NativeArray<TriggerMeta>;
	public var GenderW:Bool;
}

@:nativeGen
@:strict(SerializableAttribute)
class GroupMeta {
	public var Id:String;
	public var Cards:NativeArray<TriggerMeta>;
}

@:nativeGen
@:strict(SerializableAttribute)
class ItemMeta {
	public var Name:String;
	public var Tags:NativeArray<String>;
	public var Particle:Int;
	public var Desc:String;
	public var Image:String;
	public var Id:String;

	public var Hidden:Bool;
	public var Type:Int;

	public var HowTo:NativeArray<ConditionMeta>;
	public var WhereTo:NativeArray<ConditionMeta>;
	public var Price:Int;
}

@:nativeGen
@:strict(SerializableAttribute)
class ChoiceMeta {
	public var Text:String;
	/*public List<RewardMeta> GetCost()
		{
			return new List<RewardMeta>();//reward != null && reward.Count > 0 ? reward.FindAll(r => r.count < 0) : new List<RewardData>();
		}
		public List<RewardMeta> GetReward()
		{
			return new List<RewardMeta>();//reward != null && reward.Count > 0 ? reward.FindAll(r => r.count > 0) : new List<RewardData>();
	}*/
}

@:nativeGen
@:strict(SerializableAttribute)
class TriggerActionMeta {
	public var Id:String;
	public var Reward:NativeArray<NativeArray<RewardMeta>>;
	public var Cost:NativeArray<NativeArray<RewardMeta>>;
	public var Next:NativeArray<TriggerMeta>;
	public var Cards:NativeArray<TriggerMeta>;
}

@:nativeGen
@:strict(SerializableAttribute)
class TriggerMeta {
	public static inline var CARD:Int = 1;
	public static inline var ITEM:Int = 2;

	public static inline var ALWAYS:Int = 10;
	public static inline var SWIPE:Int = 11;
	public static inline var START_GAME:Int = 12;
	public static inline var EVENT:Int = 13;
	public static inline var CHANGE_LOCATION:Int = 14;
	public static inline var REROLL:Int = 15;
	public static inline var CHOOSE_SELF_HERO:Int = 16;
	public static inline var TUTORIAL:Int = 17;

	public var Id:String;
	public var Type:Int;
	public var Count:Int;
	public var Tags:NativeArray<String>;
	public var Value:Int;
	public var Chance:Int;
	public var Next:String;
	public var IfNot:String;
	public var Over:String;
}

@:nativeGen
@:strict(SerializableAttribute)
class ConditionMeta {
	public static inline var ANY:Int = 0;
	public static inline var CARD:Int = 1;
	public static inline var ITEM:Int = 2;
	public static inline var LOCATION:Int = 3;
	public static inline var QUEST:Int = 4;
	public static inline var SKILL:Int = 5;

	// public static inline var CARD_COND:Int = 6;
	public var Id:String;
	public var Type:Int;
	public var Tags:NativeArray<String>;
	// public var Invert:Bool;
	public var Sign:String;
	public var Choice:Int;
	public var Count:Int;
	public var Loc:NativeArray<Int>;
}

@:nativeGen
@:strict(SerializableAttribute)
class BuffMeta {
	public var Id:String;
	public var Reward:NativeArray<NativeArray<RewardMeta>>;
	public var Cost:NativeArray<NativeArray<RewardMeta>>;
	// public var Cards:NativeArray<TriggerMeta>;
	// public var ApplyR:NativeArray<NativeArray<RewardMeta>>;
	// public var ApplyC:NativeArray<NativeArray<RewardMeta>>;
	public var Con:NativeArray<NativeArray<ConditionMeta>>;
}

@:nativeGen
@:strict(SerializableAttribute)
class CardMeta {
	public static inline var LEFT:Int = 0;
	public static inline var RIGHT:Int = 1;

	public static inline var ACTIVATED:Int = 0;
	public static inline var EXECUTED:Int = 1;

	public static inline var TYPE_CARD:Int = 0;
	public static inline var TYPE_SKILL:Int = 1;
	public static inline var TYPE_QUEST:Int = 2;
	public static inline var TYPE_GROUP:Int = 3;

	public static inline var QUEST_ACTIVE:Int = 0;
	public static inline var QUEST_SUCCESS:Int = 1;
	public static inline var QUEST_FAIL:Int = 2;

	public var Id:String;

	public var CT:Int;
	public var CR:Int;
	public var Chance:Int;
	public var Hero:String;
	public var Type:Int;
	public var Cut:Bool;
	public var Shake:Bool;
	public var Level:Int;
	public var Call:Bool;

	public var CN:String;

	public var Name:String;
	public var Descs:NativeArray<String>;
	public var IfNothing:NativeArray<String>;
	public var OnlyOnce:NativeArray<String>;
	public var RewardText:NativeArray<String>;
	public var RStory:Bool;
	public var Shure:String;
	public var OneNext:Bool;
	public var Quest:Bool;

	public var Image:String;

	public var Reward:NativeArray<NativeArray<RewardMeta>>;
	public var Cost:NativeArray<NativeArray<RewardMeta>>;
	public var Act:String;

	public var Next:NativeArray<TriggerMeta>;
	public var Over:NativeArray<TriggerMeta>;
	public var IfNot:NativeArray<TriggerMeta>;
	public var IfWin:NativeArray<TriggerMeta>;

	// public var Triggered:NativeArray<TriggerMeta>;
	public var TradeLimit:Int;

	public var Pri:Int;
	public var Con:NativeArray<NativeArray<ConditionMeta>>;
	public var Text:String;

	public var Sound:NativeArray<String>;
}
