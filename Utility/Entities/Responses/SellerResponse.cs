using System;

namespace Utility.Entities.Responses
{
    public class SellerResponse
    {
        public bool Success { get; set; }
        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }
        public Seller Seller { get; set; }

        public SellerResponse(Seller seller)
        {
            Success = true;
            Seller = seller;
        }

        public SellerResponse(bool success, string message)
        {
            Success = success;
            if (success) SuccessMessage = message;
            else ErrorMessage = message;
        }

        public SellerResponse(bool success, string message, Seller seller)
        {
            Success = success;
            if (success) SuccessMessage = message;
            else ErrorMessage = message;
            Seller = seller;
        }
    }
}