using Chat.Common.Models;
using Chat.Service.Services;
using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Chat.Test
{
    public class Tests
    {
        public TeamModel team { get; private set; }
        public List<ChatModel> chats { get; private set; }

        [SetUp]
        public void Setup()
        {
            team = new TeamModel
            {
                Agents = new List<AgentModel>
                {
                    new AgentModel(1,"Mid1",Level.MidLevel),
                    new AgentModel(2,"Jr1",Level.Junior),
                    new AgentModel(3,"Mid2",Level.MidLevel),
                    new AgentModel(4,"Sern1",Level.Senior),
                    new AgentModel(5,"Jr2",Level.Junior),
                }
            };

            chats = new List<ChatModel>
            {
                new ChatModel { Id = 1, Message = "Chat Message 1"},
                new ChatModel { Id = 2, Message = "Chat Message 2"},
                new ChatModel { Id = 3, Message = "Chat Message 3"},
                new ChatModel { Id = 4, Message = "Chat Message 4"},
                new ChatModel { Id = 5, Message = "Chat Message 5"},
                new ChatModel { Id = 6, Message = "Chat Message 6"},
                new ChatModel { Id = 7, Message = "Chat Message 7"},
                new ChatModel { Id = 8, Message = "Chat Message 8"},
                new ChatModel { Id = 9, Message = "Chat Message 9"},
                new ChatModel { Id = 10, Message = "Chat Message 10"},
                new ChatModel { Id = 11, Message = "Chat Message 11"},
                new ChatModel { Id = 12, Message = "Chat Message 12"},
                new ChatModel { Id = 13, Message = "Chat Message 13"},
                new ChatModel { Id = 14, Message = "Chat Message 14"},
                new ChatModel { Id = 15, Message = "Chat Message 15"},
            };
        }

        [Test]
        public void TestQueueCapacity()
        {
            var chatService = new ChatService(null);
            var capacity = chatService.GetTeamCapacity(team);

            Assert.AreEqual(31,capacity);
        }

        [Test]
        public void TestTeamAssign()
        {
            var chatService = new ChatService(null);
            
            foreach (var chat in chats)
            {
                chatService.AssignChat(chat, team);
                foreach (var agent in team.Agents)
                {
                    Debug.WriteLine($"{agent.Id} {agent.Name} {agent.Level.ToString()} {agent.Queue.Count}");                  
                }
                Debug.WriteLine("=======================================================================");
            }

            Assert.Pass();
        }

    }
}