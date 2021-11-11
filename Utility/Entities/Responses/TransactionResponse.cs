using System;
using System.Collections.Generic;

namespace Utility.Entities.Responses
{
    public class TransactionResponse
    {
        public bool Success { get; set; }
        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }
        public Invoice Bill { get; set; }
        public Transaction Transct { get; set; }
        public List<Transaction> Transactions { get; set; }

        public TransactionResponse(bool success, string message)
        {
            Success = success;
            if (success) SuccessMessage = message;
            else ErrorMessage = message;
            Bill = null;
            Transct = null;
            Transactions = null;
        }

        public TransactionResponse(bool success, string message, Invoice bill)
        {
            Success = success;
            if (success) SuccessMessage = message;
            else ErrorMessage = message;
            Bill = bill;
            Transct = null;
            Transactions = null;
        }
        public TransactionResponse(bool success, string message, Transaction transaction)
        {
            Success = success;
            if (success) SuccessMessage = message;
            else ErrorMessage = message;
            Bill = null;
            Transct = transaction;
            Transactions = null;
        }

        public TransactionResponse(bool success, string message, List<Transaction> transactions)
        {
            Success = success;
            if (success) SuccessMessage = message;
            else ErrorMessage = message;
            Bill = null;
            Transct = null;
            Transactions = transactions;
        }
        
    }
}