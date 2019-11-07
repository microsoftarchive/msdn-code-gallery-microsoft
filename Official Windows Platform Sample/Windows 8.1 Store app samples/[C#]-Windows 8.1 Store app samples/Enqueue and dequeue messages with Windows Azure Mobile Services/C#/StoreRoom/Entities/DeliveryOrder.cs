//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

namespace StoreRoom.Entities
{
    using System.Runtime.Serialization;

    [DataContract(Name = "DeliveryOrder")]
    public class DeliveryOrder
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "product")]
        public string Product { get; set; }

        [DataMember(Name = "quantity")]
        public int Quantity { get; set; }

        [DataMember(Name = "customer")]
        public string Customer { get; set; }

        [DataMember(Name = "delivered")]
        public bool Delivered { get; set; }
    }
}
