using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ServerCore;

public enum SessionType
{
    Login = 0,
    Room = 1,
    InGame = 2,
}

public class SessionFactory : Singleton<SessionFactory>
{
    Dictionary<SessionType, Session> sessions = new Dictionary<SessionType, Session>();
    object lockObj = new object();

    public Session Make(SessionType type)
    {
        lock (lockObj)
        {
            var session = MakeInternal(type);
            if (session == null)
                return null;

            sessions[type] = session;

            return session;
        }
    }

    private Session MakeInternal(SessionType type)
    {
        switch (type)
        {
            case SessionType.InGame:
                return new InGameSession((int)type);
        }

        return null;
    }

    public void Remove(SessionType type)
    {
        lock (lockObj)
        {
            sessions.Remove(type);
        }
    }
}