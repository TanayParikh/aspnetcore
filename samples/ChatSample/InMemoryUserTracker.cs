﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Sockets;

namespace ChatSample
{
    public class InMemoryUserTracker<THub> : IUserTracker<THub>
    {
        private readonly ConcurrentDictionary<Connection, UserDetails> _usersOnline
            = new ConcurrentDictionary<Connection, UserDetails>();

        public event Action<UserDetails> UserJoined;
        public event Action<UserDetails> UserLeft;

        public Task<IEnumerable<UserDetails>> UsersOnline()
            => Task.FromResult(_usersOnline.Values.AsEnumerable());

        public Task AddUser(Connection connection, UserDetails user)
        {
            _usersOnline.TryAdd(connection, user);
            UserJoined(user);

            return Task.CompletedTask;
        }

        public Task<UserDetails> RemoveUser(Connection connection)
        {
            if (_usersOnline.TryRemove(connection, out var userDetails))
            {
                UserLeft(userDetails);
            }

            return Task.FromResult(userDetails);
        }
    }
}
