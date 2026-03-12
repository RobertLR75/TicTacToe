using System.Text.Json;

namespace TicTacToeMud.Session;

public static class SessionUserStore
{
    public const string SessionKey = "session_user";

    public static bool TryRead(HttpContext context, out SessionUser sessionUser)
    {
        return TryRead(context.Session, out sessionUser);
    }

    public static bool TryRead(ISession session, out SessionUser sessionUser)
    {
        var payload = session.GetString(SessionKey);
        if (string.IsNullOrWhiteSpace(payload))
        {
            sessionUser = default;
            return false;
        }

        try
        {
            var parsed = JsonSerializer.Deserialize<SessionUser>(payload);
            if (parsed.UserId == Guid.Empty || string.IsNullOrWhiteSpace(parsed.Name))
            {
                sessionUser = default;
                return false;
            }

            sessionUser = new SessionUser(parsed.UserId, parsed.Name);
            return true;
        }
        catch (JsonException)
        {
            sessionUser = default;
            return false;
        }
    }

    public static void Write(ISession session, SessionUser sessionUser)
    {
        session.SetString(SessionKey, JsonSerializer.Serialize(sessionUser));
    }

    public static void Clear(ISession session)
    {
        session.Remove(SessionKey);
    }
}
