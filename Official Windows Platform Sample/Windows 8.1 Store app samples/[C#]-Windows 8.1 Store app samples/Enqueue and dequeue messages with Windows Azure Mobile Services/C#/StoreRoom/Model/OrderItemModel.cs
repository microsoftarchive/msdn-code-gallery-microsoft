//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

namespace StoreRoom.Model
{
    public class OrderItemModel
    {
        public string Id { get; set; }

        public string Product { get; set; }
                
        public int Quantity { get; set; }
                
        public string Customer { get; set; }
                
        public bool Delivered { get; set; }
    }
}
