
using haxe.root;

public class SkillEffectBuilder
{
    public string Apply(SkillMeta meta, SkillItem data)
    {
        var text = meta.Descs[2].Localize(LocalizePartEnum.CardDescription);
        var value = 0;
        var color = "#FFFC00";

        switch (data.Id)
        {
            case "1":
                value = SkillHelper.GetSkillValue(meta.Id, null, Services.Player.Profile, Services.Meta.Game, SkillMeta.DEFAULT, 0);
                text = text.Replace("#", $"<b><color={color}>{(value / 100.0).ToString("0.##")}%</color></b>");
                break;
            case "3":
                value = SkillHelper.GetSkillValue(meta.Id, null, Services.Player.Profile, Services.Meta.Game, SkillMeta.CHANCE_MULTIPLE, 0);
                text = text.Replace("#", $"<b><color={color}>{value.ToString()}%</color></b>");
                break;

            default:
                if (meta.Values != null)
                {
                    value = SkillHelper.GetSkillValue(meta.Id, null, Services.Player.Profile, Services.Meta.Game, -1, 0);
                    text = text.Replace("#", $"<b><color={color}>{value.ToString()}</color></b>");
                }
                else if (meta.Reward != null || meta.Cost != null)
                {

                    text = text.Replace("#", "");
                }

                break;
        }

        return text;
    }

}