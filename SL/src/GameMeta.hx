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
	public var Quests:NativeArray<CardMeta>;
	public var Items:Dictionary_2<String, ItemMeta>;
	public var Skills:Dictionary_2<String, SkillMeta>;
	public var Locations:Dictionary_2<String, CardMeta>;

	public var Profile:PlayerMeta;
	public var Config:ConfigMeta;
	public var Version:Int;
}

@:nativeGen
@:strict(SerializableAttribute)
class PlayerMeta {
	public var Cards:NativeArray<Int>;
	public var Reward:NativeArray<RewardMeta>;
	public var Tags:NativeArray<String>;
	public var Locations:String;
}

@:nativeGen
@:strict(SerializableAttribute)
class ConfigMeta {
	public var DurationReroll:Int;
	public var PriceReroll:NativeArray<RewardMeta>;
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
	// public RewardMeta Clone()
	// {
	// 	RewardMeta r = new RewardMeta();
	// 	r.Id = Id;
	// 	r.Tp = Tp;
	// 	r.Tags = Tags != null ? (string[])Tags.Clone() : new string[0];
	// 	r.Chance = Chance;
	// 	r.Count = Count;
	// 	r.Random = Random != null ? (int[])Random.Clone() : new int[0];
	// 	r.Condi = Condi != null ? new List<ConditionMeta>(Condi) : null;
	// 	return r;
	// }
	// public ConditionMeta ToCondition()
	// {
	// 	ConditionMeta c = new ConditionMeta();
	// 	c.Id = Id;
	// 	c.Tp = Tp;
	// 	c.Count = Math.Abs(Count);
	// 	return c;
	// }
}

@:nativeGen
@:strict(SerializableAttribute)
class SkillMeta extends ItemMeta {
	public var Slot:Int;
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

	public var Hide:Bool;
	public var Type:Int;
}

// @:nativeGen
// @:strict(SerializableAttribute)
// class ActionMeta {
// 	public var Reward:NativeArray<RewardMeta>;
// 	public var Tri:NativeArray<TriggerMeta>;
// 	public var Con:NativeArray<ConditionMeta>;
// 	public var Text:String;
// 	public var Chance:Int;
// 	// public ActionMeta Clone()
// 	// {
// 	// 	ActionMeta action = new ActionMeta();
// 	// 	/*action.Time = Time;
// 	// 	action.Text = Text;
// 	// 	action.image = image;
// 	// 	action.Chance = Chance;
// 	// 	action.reward = new List<RewardData>(reward);
// 	// 	action.trigg = new List<TriggerData>(trigg);
// 	// 	action.Con = new List<ConditionData>(Con);
// 	// 	*/
// 	// 	return action;
// 	// }
// 	// public List<RewardMeta> GetCost()
// 	// {
// 	// 	return new List<RewardMeta>();//reward != null && reward.Count > 0 ? reward.FindAll(r => r.count < 0) : new List<RewardData>();
// 	// }
// 	// public List<RewardMeta> GetReward()
// 	// {
// 	// 	return new List<RewardMeta>();//reward != null && reward.Count > 0 ? reward.FindAll(r => r.count > 0) : new List<RewardData>();
// 	// }
// }

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
class TriggerMeta {
	public static inline var ALWAYS:Int = 0;
	public static inline var SWIPE:Int = 1;
	public static inline var START_GAME:Int = 2;
	public static inline var EVENT:Int = 3;
	public static inline var CHANGE_LOCATION:Int = 4;
	public static inline var REROLL:Int = 5;

	public var Id:String;
	public var Type:Int;
	public var Tags:NativeArray<String>;
	public var Value:Int;
}

@:nativeGen
@:strict(SerializableAttribute)
class ConditionMeta {
	public static inline var ANY:Int = 0;
	public static inline var CARD:Int = 1;
	public static inline var ITEM:Int = 2;
	public static inline var LOCATION:Int = 3;

	public var Id:String;
	public var Type:Int;
	public var Tags:NativeArray<String>;
	public var Invert:Bool;
	public var Sign:String;
	public var Choice:Int;
	public var Count:Int;
	public var Loc:NativeArray<Int>;
}

@:nativeGen
@:strict(SerializableAttribute)
class CardMeta {
	public static inline var LEFT:Int = 0;
	public static inline var RIGHT:Int = 1;

	public static inline var ACTIVATED:Int = 0;
	public static inline var EXECUTED:Int = 1;

	public var Id:String;
	public var Tags:NativeArray<String>;
	public var Pri:Int;
	public var CT:Int;
	public var CR:Int;
	public var Group:Bool;

	public var Name:String;
	public var Desc:String;
	public var Image:String;

	public var Reward:NativeArray<RewardMeta>;
	public var Next:NativeArray<TriggerMeta>;
	public var Over:NativeArray<TriggerMeta>;

	public var Con:NativeArray<ConditionMeta>;
	public var Text:String;
	public var Chance:Int;

	public var Sound:NativeArray<String>;
}
