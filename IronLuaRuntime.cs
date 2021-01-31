using CSR;
using Newtonsoft.Json.Linq;
using NLua;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace CSharpLuaRunner
{
    class CSharpLuaRunner
    {
        public static Lua lua;
        public static Dictionary<string, IntPtr> ptr = new Dictionary<string, IntPtr>();
        public static Dictionary<string, object> ObjectDatas = new Dictionary<string, object>();
        public static Dictionary<string, List<LuaFunction>> LuaFunctiON = new Dictionary<string, List<LuaFunction>>();

        public class MCLUAAPI
        {
            private MCCSAPI api { get; set; }
            private Dictionary<string, int> TPFuncPtr { get; set; }

            public MCLUAAPI(MCCSAPI api)
            {
                this.api = api;
                TPFuncPtr = new Dictionary<string, int>
                {
                    { "1.16.200.2", 0x00C82C60 },
                    { "1.16.201.2", 0x00C82C60 }
                };
            }
            #region MCLUAAPI
            public string MCCSAPIVERSION()
            {
                return api.VERSION;
            }
            public void Listen(string key, LuaFunction fun)
            {
                try
                {
                    if (!LuaFunctiON.ContainsKey(key))
                    {
                        LuaFunctiON.Add(key, new List<LuaFunction>());
                    }
                    LuaFunctiON[key].Add(fun);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            public void makeCommand(string key, string descripition)
            {
                api.setCommandDescribe(key, descripition);
            }

            public void runCmd(string cmd)
            {
                api.runcmd(cmd);
            }

            public void logout(string msg)
            {
                api.logout(msg);
            }

            public string getOnLinePlayeys()
            {
                return api.getOnLinePlayers();
            }
            public void teleport(string uuid, float x, float y, float z, int did)
            {
                IntPtr player = IntPtr.Zero;
                int _ptr = 0;
                if (TPFuncPtr.TryGetValue(api.VERSION, out _ptr) &&
                    ptr.TryGetValue(uuid, out player))
                {
                    var temp = new Vec3
                    {
                        x = x,
                        y = y,
                        z = z
                    };
                    Hook.tp(api, _ptr, player, temp, did);
                }
                else
                {
                    api.teleport(uuid, x, y, z, did);
                }
            }

            public void reNameByUuid(string uuid, string name)
            {
                api.reNameByUuid(uuid, name);
            }

            public void talkAs(string uuid, string msg)
            {
                api.talkAs(uuid, msg);
            }

            public void runCmdAs(string uuid, string command)
            {
                api.runcmdAs(uuid, command);
            }

            public void disconnectClient(string uuid, string tips)
            {
                api.disconnectClient(uuid, tips);
            }

            public void sendText(string uuid, string text)
            {
                api.sendText(uuid, text);
            }

            public uint sendSimpleForm(string uuid, string title, string contest, string buttons)
            {
                return api.sendSimpleForm(uuid, title, contest, buttons);
            }

            public uint sendModalForm(string uuid, string title, string contest, string button1, string button2)
            {
                return api.sendModalForm(uuid, title, contest, button1, button2);
            }

            public uint GUI(string uuid, string json)
            {
                return api.sendCustomForm(uuid, json);
            }

            public void releaseForm(uint formid)
            {
                api.releaseForm(formid);
            }

            public string selectPlayer(string uuid)
            {
                return api.selectPlayer(uuid);
            }

            public void addPlayerItem(string uuid, int id, short aux, byte count)
            {
                api.addPlayerItem(uuid, id, aux, count);
            }

            public int getscoreboard(string uuid, string name)
            {
                return api.getscoreboard(uuid, name);
            }

            public void setscoreboard(string uuid, string objname, int count)
            {
                api.setscoreboard(uuid, objname, count);
            }

            public void setServerMotd(string motd, bool isshow)
            {
                api.setServerMotd(motd, isshow);
            }

            public string getPlayerAbilities(string uuid)
            {
                return api.getPlayerAbilities(uuid);
            }

            public string getPlayerAttributes(string uuid)
            {
                return api.getPlayerAttributes(uuid);
            }

            public string getPlayerMaxAttributes(string uuid)
            {
                return api.getPlayerMaxAttributes(uuid);
            }

            public string dumpInv(string uuid)
            {
                return api.getPlayerItems(uuid);
            }

            public string getPlayerSelectedItem(string uuid)
            {
                return api.getPlayerSelectedItem(uuid);
            }

            public string getPlayerEffects(string uuid)
            {
                return api.getPlayerEffects(uuid);
            }

            public void setPlayerBossBar(string uuid, string title, float percent)
            {
                api.setPlayerBossBar(uuid, title, percent);
            }

            public void removePlayerBossBar(string uuid)
            {
                api.removePlayerBossBar(uuid);
            }

            public void transferserver(string uuid, string addr, int port)
            {
                api.transferserver(uuid, addr, port);
            }

            public void setPlayerSidebar(string uuid, string title, string list)
            {
                api.setPlayerSidebar(uuid, title, list);
            }

            public void removePlayerSidebar(string uuid)
            {
                api.removePlayerSidebar(uuid);
            }

            public string getPlayerPermissionAndGametype(string uuid)
            {
                return api.getPlayerPermissionAndGametype(uuid);
            }

            public void setPlayerPermissionAndGametype(string uuid, string modes)
            {
                api.setPlayerPermissionAndGametype(uuid, modes);
            }

            public CsPlayer creatPlayerObject(string uuid)
            {
                try
                {
                    var pl = ptr[uuid];
                    return new CsPlayer(api, pl);
                }
                catch (Exception e)
                {
                    Console.WriteLine("[ILUAR] " + e.Message);
                    return null;
                }
            }

            public CsActor getActorFromUniqueid(ulong uniqueid)
            {
                return CsActor.getFromUniqueId(api, uniqueid);
            }

            public CsPlayer getPlayerFromUniqueid(ulong uniqueid)
            {
                return (CsPlayer)CsPlayer.getFromUniqueId(api, uniqueid);
            }

            public CsActor[] getFromAABB(int did, float x1, float y1, float z1, float x2, float y2, float z2)
            {
                var temp = new List<CsActor>();
                var raw = CsActor.getsFromAABB(api, did, x1, y1, z2, x2, y2, z2);
                foreach (var i in raw)
                {
                    temp.Add((CsActor)i);
                }
                return temp.ToArray();
            }

            public CsPlayer convertActorToPlayer(CsActor ac)
            {
                return (CsPlayer)ac;
            }
            public string GetUUID(string playername) 
            {
                var json = JArray.Parse(api.getOnLinePlayers());
                foreach (var i in json) 
                {
                    if (i["playername"].ToString() == playername)
                        return i["uuid"].ToString();
                }
                return null;
            }
            #endregion

        }

        public static bool TRY(Action a)
        {
            try
            {
                a();
                return true;
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ILUAR] " + e.Message);
                Console.ForegroundColor = ConsoleColor.White;
                return false;
            }
        }

        public static bool CallLuaFunc<T>(string key, T data)
        {
            bool re = true;
            if (LuaFunctiON.ContainsKey(key))
            {
                foreach (var fun in LuaFunctiON[key])
                {
                    TRY(() =>
                    {
                        var tmp = Convert.ToBoolean(fun.Call(data)[0] ?? true);
                        if (re && !tmp)
                            re = false;
                    });
                }
            }
            return re;
        }

        static string CsGetUuid(List<IntPtr> pls, string pln, MCCSAPI api)
        {
            foreach (IntPtr pl in pls)
            {
                CsPlayer cpl = new CsPlayer(api, pl);
                if (cpl.getName() == pln)
                {
                    return cpl.Uuid;
                }
            }
            return string.Empty;
        }
        public static void RunLua(MCCSAPI api)
        {
            Console.ForegroundColor = ConsoleColor.White;
            List<IntPtr> uuid = new List<IntPtr>();
            const String path = "./ilua";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            lua = new Lua();
            var mc = new MCLUAAPI(api);
            var tool = new CSLR.ToolFunc();
            lua["tool"] = tool;
            lua["mc"] = mc;
            lua.LoadCLRPackage();
            DirectoryInfo Allfolder = new DirectoryInfo(path);
            Console.WriteLine("[ILUAR] Load! version=1.3.0");
            Console.WriteLine("[ILUAR] 本平台基于AGPL发行。");
            Console.WriteLine("[ILUAR] Reading Plugins...");            
            foreach (FileInfo file in Allfolder.GetFiles("*.net.lua"))
            {
                try
                {
                    lua.DoFile(file.FullName);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("[ILUAR] " + file.Name + "加载成功");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[ILUAR] " + e.Message);
                    Console.WriteLine("[ILUAR] Filed to load " + file.Name);
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }

            api.addBeforeActListener(EventKey.onLoadName, x =>
            {
                var a = BaseEvent.getFrom(x) as LoadNameEvent;
                ptr.Add(a.playername, a.playerPtr);
                CallLuaFunc("onLoadName", a);
                return true;
            });

            api.addBeforeActListener(EventKey.onPlayerLeft, x =>
            {
                var a = BaseEvent.getFrom(x) as PlayerLeftEvent;
                ptr.Remove(a.playername);
                CallLuaFunc("onPlayerLeft", a);
                return true;
            });
            api.addBeforeActListener(EventKey.onInputCommand, x =>
            {
                var a = BaseEvent.getFrom(x) as InputCommandEvent;
                return CallLuaFunc("onInputCommand", a);
            });
            api.addBeforeActListener(EventKey.onLevelExplode, x =>
            {
                var a = BaseEvent.getFrom(x) as LevelExplodeEvent;
                return CallLuaFunc("onLevelExplode", a);
            });
            api.addBeforeActListener(EventKey.onMobDie, x =>
            {
                var a = BaseEvent.getFrom(x) as MobDieEvent;
                CallLuaFunc("onMobDie", a);
                return true;
            });
            api.addBeforeActListener(EventKey.onPlacedBlock, x =>
            {
                var a = BaseEvent.getFrom(x) as PlacedBlockEvent;
                return CallLuaFunc("onPlacedBlock", a);
            });
            api.addBeforeActListener(EventKey.onDestroyBlock, x =>
            {
                var a = BaseEvent.getFrom(x) as DestroyBlockEvent;
                return CallLuaFunc("onDestroyBlock", a);
            });
            api.addBeforeActListener(EventKey.onFormSelect, x =>
            {
                var a = BaseEvent.getFrom(x) as FormSelectEvent;
                return CallLuaFunc("onFormSelect", a);
            });
            api.addBeforeActListener(EventKey.onInputText, x =>
            {
                var a = BaseEvent.getFrom(x) as InputTextEvent;
                return CallLuaFunc("onInputText", a);
            });
            api.addBeforeActListener(EventKey.onScoreChanged, x =>
            {
                var a = BaseEvent.getFrom(x) as ScoreChangedEvent;
                CallLuaFunc("onScoreChanged", a);
                return true;
            });
            api.addBeforeActListener(EventKey.onServerCmd, x =>
            {
                var a = BaseEvent.getFrom(x) as ServerCmdEvent;
                return CallLuaFunc("onServerCmd", a);
            });
            api.addBeforeActListener(EventKey.onServerCmdOutput, x =>
            {
                var a = BaseEvent.getFrom(x) as ServerCmdOutputEvent;
                return CallLuaFunc("onServerCmdOutput", a);
            });
            api.addBeforeActListener(EventKey.onRespawn, x =>
            {
                var a = BaseEvent.getFrom(x) as RespawnEvent;
                return CallLuaFunc("onRespawn", a);
            });
        }
    }
}

namespace CSR
{
    partial class Plugin
    {
        public static void onStart(MCCSAPI api)
        {
            CSharpLuaRunner.CSharpLuaRunner.RunLua(api);
            Console.WriteLine("[ILUAR] IronLuaRunner加载完成");
        }
    }
}
