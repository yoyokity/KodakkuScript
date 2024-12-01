using System;
using System.Numerics;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Script;
using KodakkuAssist.Module.GameEvent.Struct;
using KodakkuAssist.Module.Draw;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.GameHelpers;
using ECommons.Logging;

namespace YoyoScriptNamespace
{
    [ScriptType(name: "舞娘技能范围",
        note: "舞娘自身技能基础范围提示：\n" +
              "大舞：15米攻击范围 + 30米团辅范围\n" +
              "小舞：15米攻击范围\n" +
              "提拉纳：15米攻击范围\n" +
              "即兴表演：8米治疗范围\n" +
              "前冲步位置：进入战斗后在落点位置永久显示一个小圆圈",
        territorys: [], guid: "2e3e02fd-7504-b11a-9cdb-fb4f58249158",
        version: "0.0.2",
        author: "yoyokity")]
    public class 舞娘技能范围
    {
        Vector4 大舞颜色 = new Vector4(1, 1, 1, .5f);
        Vector4 团辅颜色 = new Vector4(1, 1, 1, .2f);
        Vector4 小舞颜色 = new Vector4(1, 1, 1, .3f);

        public void Init(ScriptAccessory accessory)
        {
        }

        [ScriptMethod(name: "前冲步位置", eventType: EventTypeEnum.CombatChanged, eventCondition: ["InCombat:True"])]
        public void 前冲步位置(Event @event, ScriptAccessory accessory)
        {
            if (Player.Job != Job.DNC)
                return;
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "前冲步位置";
            dp.Scale = new Vector2(.2f);
            dp.Offset = new Vector3(0, 0, -10);
            dp.Color = accessory.Data.DefaultSafeColor;
            dp.Owner = (uint)Svc.ClientState.LocalPlayer.GameObjectId;
            dp.DestoryAt = 99999 * 9999;
            accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Circle, dp);
        }

        [ScriptMethod(name: "大舞范围", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:15998"])]
        public void 大舞范围(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            if (sid != Svc.ClientState.LocalPlayer.GameObjectId) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "大舞范围";
            dp.Scale = new Vector2(15);
            dp.Color = 大舞颜色;
            dp.Owner = sid;
            dp.DestoryAt = 15 * 1000;
            accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Circle, dp);

            var dp2 = accessory.Data.GetDefaultDrawProperties();
            dp2.Name = "大舞团辅范围";
            dp2.Scale = new Vector2(30);
            dp2.InnerScale = new Vector2(15);
            dp2.Color = 团辅颜色;
            dp2.Owner = sid;
            dp2.DestoryAt = 15 * 1000;
            accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Donut, dp2);
        }

        [ScriptMethod(name: "小舞范围", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:15997"])]
        public void 小舞范围(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            if (sid != Svc.ClientState.LocalPlayer.GameObjectId) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "小舞范围";
            dp.Scale = new Vector2(15);
            dp.Color = 小舞颜色;
            dp.Owner = sid;
            dp.DestoryAt = 15 * 1000;
            accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Circle, dp);
        }

        [ScriptMethod(name: "提拉纳范围", eventType: EventTypeEnum.StatusAdd, eventCondition: ["StatusID:2698"])]
        public void 提拉纳范围(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            if (sid != Svc.ClientState.LocalPlayer.GameObjectId) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "提拉纳范围";
            dp.Scale = new Vector2(15);
            dp.Color = 小舞颜色;
            dp.Owner = sid;
            dp.DestoryAt = 30 * 1000;
            accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Circle, dp);
        }

        [ScriptMethod(name: "即兴表演范围", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:16014"])]
        public void 即兴表演范围(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            if (sid != Svc.ClientState.LocalPlayer.GameObjectId) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "即兴表演范围";
            dp.Scale = new Vector2(8);
            dp.Color = accessory.Data.DefaultSafeColor;
            dp.Owner = sid;
            dp.DestoryAt = 15 * 1000;
            accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Circle, dp);
        }

        //
        [ScriptMethod(name: "前冲步位置移除", eventType: EventTypeEnum.CombatChanged, eventCondition: ["InCombat:False"])]
        public void 前冲步位置移除(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.RemoveDraw("前冲步位置");
        }

        [ScriptMethod(name: "大舞范围移除", eventType: EventTypeEnum.StatusRemove, eventCondition: ["StatusID:1819"])]
        public void 大舞范围移除(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            if (sid != Svc.ClientState.LocalPlayer.GameObjectId) return;

            accessory.Method.RemoveDraw("大舞范围");
            accessory.Method.RemoveDraw("大舞团辅范围");
        }

        [ScriptMethod(name: "小舞范围移除", eventType: EventTypeEnum.StatusRemove, eventCondition: ["StatusID:1818"])]
        public void 小舞范围移除(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            if (sid != Svc.ClientState.LocalPlayer.GameObjectId) return;

            accessory.Method.RemoveDraw("小舞范围");
        }

        [ScriptMethod(name: "提拉纳范围移除", eventType: EventTypeEnum.StatusRemove, eventCondition: ["StatusID:2698"])]
        public void 提拉纳范围移除(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            if (sid != Svc.ClientState.LocalPlayer.GameObjectId) return;

            accessory.Method.RemoveDraw("提拉纳范围");
        }

        [ScriptMethod(name: "即兴表演范围移除", eventType: EventTypeEnum.StatusRemove, eventCondition: ["StatusID:1827"])]
        public void 即兴表演范围移除(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            if (sid != Svc.ClientState.LocalPlayer.GameObjectId) return;

            accessory.Method.RemoveDraw("即兴表演范围");
        }


        private static bool ParseObjectId(string? idStr, out uint id)
        {
            id = 0;
            if (string.IsNullOrEmpty(idStr)) return false;
            try
            {
                var idStr2 = idStr.Replace("0x", "");
                id = uint.Parse(idStr2, System.Globalization.NumberStyles.HexNumber);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}