import haxe.macro.Expr.Catch;
import haxe.rtti.CType.CTypeTools;
import haxe.rtti.Meta;
import haxe.macro.Expr.Error;
import haxe.Int64;
import haxe.Json;
import GameMeta;
import seedyrng.Random;
import GameData;
import haxe.macro.Context;
import SL;

using SL.CSExtension;

#if cs
import cs.system.collections.generic.Dictionary_2;
import cs.system.collections.generic.List_1;
import cs.NativeArray;
#else
#end
class SkillHelper {
	public static function GetSkill(id:String, triggers:NativeArray<TriggerMeta>, tags:NativeArray<String>, ?type:Int, meta:GameMeta,
			profile:ProfileData):SkillTypeDef {
		if (id != null) {
			var skill = profile.Skills.tryGet(id);
			if (skill == null)
				return null;
			if (type != -1 && skill.Type != type)
				return null;
			return {meta: meta.Skills.tryGet(id), profile: skill};
		}

		if (tags != null) {
			for (s in profile.Skills.GetValues()) {
				if (type != null && s.Type != type)
					continue;
				var m = meta.Skills.tryGet(s.Id);
				if (m.Tags != null && m.Tags.intersect(tags))
					return {meta: m, profile: s};
			}
		}
		if (triggers != null)
			for (t in triggers)
				if (t.Type == TriggerMeta.SKILL) {
					var s:SkillItem = profile.Skills.tryGet(t.Id);
					if (s != null && s.Type == type)
						return {meta: meta.Skills.tryGet(s.Id), profile: s};
				}
		return null;
	}

	public static function GetSkillValue(id:String, tags:NativeArray<String>, profile:ProfileData, meta:GameMeta, ?type:Int, defaultValue:Int):Int {
		if (tags != null) {
			for (s in profile.Skills.GetValues()) {
				if (type != null && s.Type != type)
					continue;
				var m = meta.Skills.tryGet(s.Id);
				if (m.Tags != null && m.Tags.intersect(tags))
					return m.Values[s.Level];
			}
			return defaultValue;
		}
		if (id == null)
			return defaultValue;

		var skill = profile.Skills.tryGet(id);
		if (skill == null)
			return defaultValue;
		if (type != -1 && skill.Type != type)
			return defaultValue;
		var skillMeta = meta.Skills.tryGet(id);
		return skillMeta.Values[skill.Level];
	}
}
