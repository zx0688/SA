
using haxe.root;

public class SkillEffectBuilder
{
    public string Apply(SkillMeta meta, SkillItem data)
    {
        var text = meta.Descs[2].Localize(LocalizePartEnum.CardDescription);
        var value = SL.GetSkillValue(meta.Id, Services.Player.Profile, Services.Meta.Game, 0);

        switch (data.Id)
        {
            case "1":
                text = text.Replace("#", (value / 100.0).ToString("0.##"));
                break;

            default:
                text = text.Replace("#", value.ToString());
                break;
        }

        return text;
    }

}