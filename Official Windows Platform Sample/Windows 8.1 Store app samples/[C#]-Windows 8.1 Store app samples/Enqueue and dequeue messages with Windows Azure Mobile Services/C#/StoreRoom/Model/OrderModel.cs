//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

namespace StoreRoom.Model
{
    using System.Collections.Generic;

    public class OrderModel
    {
        public OrderModel()
        {
            this.Items = new List<OrderItemModel>();
        }

        public IList<OrderItemModel> Items { get; set; }
    }    
}
