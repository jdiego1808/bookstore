using System;
using System.Collections.Generic;

namespace Utility.Entities.Responses
{
    public class StationeryResponse
    {
        public List<Stationery> Stationeries { get; set; }
        public int Count { get; set; }
        public Stationery Stationery { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }

        public StationeryResponse(bool success, string message)
        {
            Success = false;
            Message = string.Empty;
            Stationeries = new List<Stationery>();
            Count = 0;
            Stationery = new Stationery();
        }

        public StationeryResponse(bool success, string message, List<Stationery> stationeries)
        {
            Success = success;
            Message = message;
            Stationeries = stationeries;
            Count = stationeries.Count;
            Stationery = new Stationery();
        }

        public StationeryResponse(bool success, string message, Stationery stationery)
        {
            Success = success;
            Message = message;
            Stationery = stationery;
            Stationeries = new List<Stationery>();
            Count = 0;
        }
    }
}