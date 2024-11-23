using System.Numerics;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Script;
using KodakkuAssist.Module.Draw;
using ECommons.DalamudServices;

namespace YoyoScriptNamespace;

[ScriptType(name: "基础画图",
    note: "进入战斗后，以自己为圆心画个小红点",
    territorys: [], guid: "cbf3972f-08ec-c9b2-a299-8ad81c9f7db3",
    version: "0.0.1",
    author: "yoyokity")]
public class 基础画图
{
    Vector4 红点颜色 = new(1, 0, 0, 1);
    
    public void Init(ScriptAccessory accessory)
    {
    }
    
    [ScriptMethod(name: "脚下红点", eventType: EventTypeEnum.CombatChanged, eventCondition: ["InCombat:True"])]
    public void 脚下红点(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "脚下红点";
        dp.Scale = new Vector2(.05f);
        dp.Color = 红点颜色;
        dp.Owner = (uint)Svc.ClientState.LocalPlayer.GameObjectId;
        dp.DestoryAt = 99999 * 9999;
        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Circle, dp);
    }
    
    [ScriptMethod(name: "脚下红点移除", eventType: EventTypeEnum.CombatChanged, eventCondition: ["InCombat:False"])]
    public void 脚下红点移除(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.RemoveDraw("脚下红点");
    }
}