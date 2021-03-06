﻿using System;
using System.Collections.Generic;
using System.Linq;
using DirittoMigrantiAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DirittoMigrantiAPI.Controllers
{
    public class MessageExchangesController : Controller, IConversationHandler, IConversationController
    {
        private readonly DbSet<MessageExchange> messageExchanges;

        public MessageExchangesController(DbSet<MessageExchange> messageExchanges)
        //è lecito che vengano passati dal costruttore?
        {
            this.messageExchanges = messageExchanges;
        }

        #region CONVERSATION CONTROLLER        
        public MessageExchange NewConversation(Message message)
        {
            try
            {
                MessageExchange conversation = new MessageExchange(message);
                messageExchanges.Add(conversation);

                return conversation;
            }
            catch (ArgumentException) { return null; }
        }
        
        public MessageExchange GetMessageExchange(long MessageExchangeId)
        {
            return messageExchanges.Find(MessageExchangeId);
            //return messageExchanges.First(mn => mn.Id == MessageExchangeId);
        }

        public bool AddMessageToConversation(long MessageExchangeId, Message message)
        {
            //TODO controllare chi lo chiama
            return GetMessageExchange(MessageExchangeId).AddMessage(message);
        }

        public string GetNotes(long id)
        {
            return GetMessageExchange(id).Notes;
        }

        public string EditNotesInConversation(long MessageExchangeId, string notes)
        {
            return GetMessageExchange(MessageExchangeId).EditNotes(notes);
        }
        #endregion

        #region CONVERSATION HANDLER        
        public List<MessageExchange> GetConversationsByUser(User user)
        {
            return messageExchanges.Where((conversation) => conversation.IsThisUserInTheConversation(user)).ToList();
        }
        
        public List<MessageExchange> GetConversationsByOwner(User user)
        {
            if (user is Consultant) return null;

            return messageExchanges.Where((conversation) => conversation.IsThisUserTheOwner(user)).ToList();
        }

        public List<MessageExchange> GetAllMessageExchangesOrderByLastUpdate()
        {
            return messageExchanges.OrderBy((conversation) => conversation.GetLastUpdate()).ToList();
            //.ThenBy() starred by user
        }

        public List<MessageExchange> GetAllMessageExchangeByCreationDate()
        {
            throw new NotImplementedException();
        }      
        #endregion

        //LOG
        protected void Log(string message, User user){
            throw new NotImplementedException();
        }

        //Qui molto probabilmente dovrebbe essere "Star"
        public Consultant StartConversation(long conversationId)
        {
            throw new NotImplementedException();
        }

        public List<MessageExchange> GetstarredConversations()
        {
            throw new NotImplementedException();
        }

    }
}
