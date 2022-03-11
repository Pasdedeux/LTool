using System;


namespace ET
{
    public static class LoginHelper
    {
        public static async ETTask<int> Login(Scene zoneScene, string address, string account, string password)
        {
            A2C_TestLoginAccount a2C_TestLoginAccount = null;
            Session session = null;

            try
            {
                session = zoneScene.GetComponent<NetKcpComponent>().Create(NetworkHelper.ToIPEndPoint(address));
                a2C_TestLoginAccount = (A2C_TestLoginAccount)await session.Call(new C2A_TestLoginAccount() { AccountName = account, AccountPassword = password });

            }
            catch (Exception e)
            {
                session?.Dispose();
                Log.Error(e.ToString());

                return ErrorCode.ERR_NetWorkError;
            }

            if (a2C_TestLoginAccount.Error != ErrorCode.ERR_Success)
            {
                return a2C_TestLoginAccount.Error;
            }
            else
            {
                Log.Debug("登录成功!");
            }

            zoneScene.GetComponent<SessionComponent>().Session = session;
            zoneScene.GetComponent<SessionComponent>().Session.AddComponent<PingComponent>();

            zoneScene.GetComponent<AccountInfoComponent>().Token = a2C_TestLoginAccount.Token;
            zoneScene.GetComponent<AccountInfoComponent>().AccountID = a2C_TestLoginAccount.AccountId;

            return ErrorCode.ERR_Success;

            //try
            //{
            //    // 创建一个ETModel层的Session
            //    R2C_Login r2CLogin;
            //    Session session = null;
            //    try
            //    {
            //        session = zoneScene.GetComponent<NetKcpComponent>().Create(NetworkHelper.ToIPEndPoint(address));
            //        {
            //            r2CLogin = (R2C_Login) await session.Call(new C2R_Login() { Account = account, Password = password });
            //        }
            //    }
            //    finally
            //    {
            //        session?.Dispose();
            //    }

            //    // 创建一个gate Session,并且保存到SessionComponent中
            //    Session gateSession = zoneScene.GetComponent<NetKcpComponent>().Create(NetworkHelper.ToIPEndPoint(r2CLogin.Address));
            //    gateSession.AddComponent<PingComponent>();
            //    zoneScene.AddComponent<SessionComponent>().Session = gateSession;
				
            //    G2C_LoginGate g2CLoginGate = (G2C_LoginGate)await gateSession.Call(
            //        new C2G_LoginGate() { Key = r2CLogin.Key, GateId = r2CLogin.GateId});

            //    Log.Debug("登陆gate成功!");

            //    await Game.EventSystem.PublishAsync(new EventType.LoginFinish() {ZoneScene = zoneScene});
            //}
            //catch (Exception e)
            //{
            //    Log.Error(e);
            //}
        } 


        public static async ETTask<int> GetServerInfo( Scene zoneScene )
        {
            A2C_GetServerInfos a2C_GetServerInfos = null;

            try
            {
                a2C_GetServerInfos = (A2C_GetServerInfos)await zoneScene.GetComponent<SessionComponent>().Session.Call(new C2A_GetServerInfos()
                {
                    AccountId = zoneScene.GetComponent<AccountInfoComponent>().AccountID,
                    Token = zoneScene.GetComponent<AccountInfoComponent>().Token,
                });
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return ErrorCode.ERR_NetWorkError;
            }

            if (a2C_GetServerInfos.Error != ErrorCode.ERR_Success)
            {
                return a2C_GetServerInfos.Error;
            }

            //todo 用AddChild方式创建了一个对象，并放置于Scene树中
            foreach (var item in a2C_GetServerInfos.ServerInfoProtoList )
            {
                ServerInfo serverInfo = zoneScene.GetComponent<ServerInfoComponent>().AddChild<ServerInfo>();
                serverInfo.FromMessage(item);
                zoneScene.GetComponent<ServerInfoComponent>().serverInfosList.Add(serverInfo);
            }


            await ETTask.CompletedTask;
            return ErrorCode.ERR_Success;
        }


        public static async ETTask<int> CreateRole(Scene zoneScene, string name)
        {
            A2C_CreateRole a2C_CreateRole = null;

            //服务器返回
            try
            {
                a2C_CreateRole = (A2C_CreateRole)await zoneScene.GetComponent<SessionComponent>().Session.Call(new C2A_CreateRole()
                {
                    AccountId = zoneScene.GetComponent<AccountInfoComponent>().AccountID,
                    Token = zoneScene.GetComponent<AccountInfoComponent>().Token,
                    Name = name,
                    ServerId =1// zoneScene.GetComponent<ServerInfoComponent>().CurrentServerId
                }) ;
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return ErrorCode.ERR_NetWorkError;
            }


            if (a2C_CreateRole.Error!= ErrorCode.ERR_Success)
            {
                Log.Error(a2C_CreateRole.Error.ToString());

                return a2C_CreateRole.Error;
            }

            RoleInfo newRoleInfo = zoneScene.GetComponent<RoleInfoComponent>().AddChild<RoleInfo>();
            newRoleInfo.FromMessage(a2C_CreateRole.RoleInfoProto);
            zoneScene.GetComponent<RoleInfoComponent>().roleInfos.Add(newRoleInfo);

            await ETTask.CompletedTask;
            return ErrorCode.ERR_Success;
        }


        public static async ETTask<int> GetRoles(Scene zoneScene)
        {
            A2C_GetRoles a2C_GetRoles = null;

            try
            {
                a2C_GetRoles = (A2C_GetRoles)await zoneScene.GetComponent<SessionComponent>().Session.Call(new C2A_GetRoles() 
                {
                    AccountId = zoneScene.GetComponent<AccountInfoComponent>().AccountID,
                    Token = zoneScene.GetComponent<AccountInfoComponent>().Token,
                    ServerId = zoneScene.GetComponent<ServerInfoComponent>().CurrentServerId,
                });
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return ErrorCode.ERR_NetWorkError;
            }

            if (a2C_GetRoles.Error != ErrorCode.ERR_Success)
            {
                Log.Error(a2C_GetRoles.Error.ToString());
                return a2C_GetRoles.Error;
            }

            Log.Debug("角色查询数量返回：");

            zoneScene.GetComponent<RoleInfoComponent>().roleInfos.Clear();
            foreach (var item in a2C_GetRoles.RoleInfo)
            {
                RoleInfo roleInfo = zoneScene.GetComponent<RoleInfoComponent>().AddChild<RoleInfo>();
                roleInfo.FromMessage(item);
                zoneScene.GetComponent<RoleInfoComponent>().roleInfos.Add(roleInfo);
            }

            var roleInfosList = zoneScene.GetComponent<RoleInfoComponent>().roleInfos;
            Log.Debug("查找到的角色数量：" + roleInfosList.Count.ToString());

            return ErrorCode.ERR_Success;
        }


        public static async ETTask<int> DeleteRole( Scene zoneScene )
        {
            A2C_DeleteRole a2C_DeleteRole = null;

            try
            {
                a2C_DeleteRole = (A2C_DeleteRole ) await zoneScene.GetComponent<SessionComponent>().Session.Call(new C2A_DeleteRole() 
                {
                    Token = zoneScene.GetComponent<AccountInfoComponent>().Token,
                    AccountId = zoneScene.GetComponent<AccountInfoComponent>().AccountID,
                    RoleInfoId = zoneScene.GetComponent<RoleInfoComponent>().CurrentRoleId,
                    ServerId = zoneScene.GetComponent<ServerInfoComponent>().CurrentServerId,
                });
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return ErrorCode.ERR_NetWorkError;
            }

            if (a2C_DeleteRole.Error != ErrorCode.ERR_Success )
            {
                Log.Error(a2C_DeleteRole.Error.ToString());
                return a2C_DeleteRole.Error;
            }

            int index = zoneScene.GetComponent<RoleInfoComponent>().roleInfos.FindIndex(e => { return e.Id == a2C_DeleteRole.DeleteRoleInfoId; });
            zoneScene.GetComponent<RoleInfoComponent>().roleInfos.RemoveAt(index);

            await ETTask.CompletedTask;
            return ErrorCode.ERR_Success;
        }


        public static async ETTask<int> EnterGame( Scene zoneScene )
        {
            string realmAddress = zoneScene.GetComponent<AccountInfoComponent>().RealmAddress;

            //发起Realm链接，获取分配的Gate

            R2C_LoginRealm r2C_LoginRealm;
            Session session = zoneScene.GetComponent<NetKcpComponent>().Create(NetworkHelper.ToIPEndPoint(realmAddress));
            try
            {
                r2C_LoginRealm = (R2C_LoginRealm)await session.Call(new C2R_LoginRealm() 
                {
                    AccountId = zoneScene.GetComponent<AccountInfoComponent>().AccountID,
                    RealmTokenKey = zoneScene.GetComponent<AccountInfoComponent>().RealmKey
                });
            }
            catch (Exception e)
            {
                Log.Error(e);
                session?.Dispose();
                return ErrorCode.ERR_NetWorkError;
            }
            
            //无论成功或者失败，都需要断开连接
            session?.Dispose();

            if (r2C_LoginRealm.Error != ErrorCode.ERR_Success)
            {
                Log.Error(r2C_LoginRealm.Error.ToString());
                return ErrorCode.ERR_NetWorkError;
            }


            Session gateSession = zoneScene.GetComponent<NetKcpComponent>().Create(NetworkHelper.ToIPEndPoint(r2C_LoginRealm.GateAddress));
            gateSession.AddComponent<PingComponent>();
            zoneScene.GetComponent<SessionComponent>().Session = gateSession;



            //开始连接Gate
            long currentRoleId = zoneScene.GetComponent<RoleInfoComponent>().CurrentRoleId;
            G2C_LogingGameGate g2C_LoginGate;
            try
            {
                long accountId = zoneScene.GetComponent<AccountInfoComponent>().AccountID;
                g2C_LoginGate = (G2C_LogingGameGate)await gateSession.Call(new C2G_LogingGameGate() { Key = r2C_LoginRealm.GateSessionKey, Account = accountId, RoleId = currentRoleId  });
                
            }
            catch (Exception e)
            {
                Log.Error(e);
                zoneScene.GetComponent<SessionComponent>().Session.Dispose();
                return ErrorCode.ERR_NetWorkError;
            }

            if (g2C_LoginGate.Error != ErrorCode.ERR_Success)
            {
                zoneScene.GetComponent<SessionComponent>().Session.Dispose();
                return ErrorCode.ERR_NetWorkError;
            }

            Log.Debug("登录Gate成功");

            //将创建角色送往逻辑服
            G2C_EnterGame g2C_EnterGame = null;
            try
            {
                g2C_EnterGame = (G2C_EnterGame)await gateSession.Call(new C2G_EnterGame() { });
            }
            catch (Exception e)
            {
                Log.Error(e);
                zoneScene.GetComponent<SessionComponent>().Session.Dispose();
                return ErrorCode.ERR_NetWorkError;
            }

            if (g2C_EnterGame.Error != ErrorCode.ERR_Success)
            {
                Log.Error(g2C_EnterGame.Error.ToString());
                return g2C_EnterGame.Error;
            }

            Log.Debug("角色进入游戏成功!");

            await ETTask.CompletedTask;
            return ErrorCode.ERR_Success;
        }

        public static async ETTask<int> GetRealmKey( Scene zoneScene )
        {
            A2C_GetRealmKey a2C_GetRealmKey = null;

            try
            {
                a2C_GetRealmKey = (A2C_GetRealmKey)await zoneScene.GetComponent<SessionComponent>().Session.Call(new C2A_GetRealmKey()
                {
                    AccountId = zoneScene.GetComponent<AccountInfoComponent>().AccountID,
                    ServerId = zoneScene.GetComponent<ServerInfoComponent>().CurrentServerId,
                    Token = zoneScene.GetComponent<AccountInfoComponent>().Token,
                }) ;
            }
            catch (Exception  e)
            {
                Log.Error(e.ToString());
                return ErrorCode.ERR_NetWorkError;
            }

            if (a2C_GetRealmKey.Error != ErrorCode.ERR_Success)
            {
                Log.Error(a2C_GetRealmKey.Error.ToString());
                return ErrorCode.ERR_NetWorkError;
            }

            zoneScene.GetComponent<AccountInfoComponent>().RealmKey = a2C_GetRealmKey.RealmKey;
            zoneScene.GetComponent<AccountInfoComponent>().RealmAddress = a2C_GetRealmKey.RealmAddress;
            zoneScene.GetComponent<SessionComponent>().Session.Dispose();

            await ETTask.CompletedTask;
            return ErrorCode.ERR_Success;
        }
    }
}