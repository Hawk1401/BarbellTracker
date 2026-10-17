using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using BarbellTracker.DomainCode;

namespace BarbellTracker.ApplicationCode
{
    public interface ISessionRepository
    {
        /// <summary>Creates or overwrites the session</summary>
        void Save(Session session);

        /// <summary>All readable sessions, newest first. Damaged files are skipped.</summary>
        IReadOnlyList<Session> GetAll();

        bool TryGet(string id, out Session session);

        /// <returns>false if there was no such session</returns>
        bool Delete(string id);
    }

    /// <summary>Stores every session as one json file in a folder</summary>
    public class JsonSessionRepository : ISessionRepository
    {
        private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

        private readonly string folder;
        private readonly object locker = new object();

        public JsonSessionRepository(string folder)
        {
            this.folder = folder ?? throw new ArgumentNullException(nameof(folder));
        }

        public void Save(Session session)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));
            var path = PathOf(session.Id);

            lock (locker)
            {
                Directory.CreateDirectory(folder);

                // write to a temp file first, so a crash does not destroy an existing session
                var temp = path + ".tmp";
                File.WriteAllText(temp, JsonSerializer.Serialize(session, Options));
                File.Move(temp, path, overwrite: true);
            }
        }

        public IReadOnlyList<Session> GetAll()
        {
            lock (locker)
            {
                if (!Directory.Exists(folder))
                {
                    return Array.Empty<Session>();
                }

                var sessions = new List<Session>();
                foreach (var file in Directory.EnumerateFiles(folder, "*.json"))
                {
                    if (TryRead(file, out var session))
                    {
                        sessions.Add(session);
                    }
                }
                return sessions.OrderByDescending(s => s.CreatedAt).ToList();
            }
        }

        public bool TryGet(string id, out Session session)
        {
            lock (locker)
            {
                var path = PathOf(id);
                session = null;
                return File.Exists(path) && TryRead(path, out session);
            }
        }

        public bool Delete(string id)
        {
            lock (locker)
            {
                var path = PathOf(id);
                if (!File.Exists(path))
                {
                    return false;
                }
                File.Delete(path);
                return true;
            }
        }

        private static bool TryRead(string file, out Session session)
        {
            try
            {
                session = JsonSerializer.Deserialize<Session>(File.ReadAllText(file));
                return session != null && !string.IsNullOrEmpty(session.Id);
            }
            catch (Exception ex) when (ex is JsonException || ex is IOException || ex is UnauthorizedAccessException)
            {
                session = null;
                return false;
            }
        }

        /// <summary>The file of a session. The id is part of a path, so only plain names are accepted.</summary>
        private string PathOf(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || id.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || id.Contains(".."))
            {
                throw new ArgumentException($"'{id}' is not a valid session id", nameof(id));
            }
            return Path.Combine(folder, id + ".json");
        }
    }
}
