﻿using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CBS
{
    public class CBSLootbox : CBSBaseItem
    {
        public List<string> RandomItemsIDs { get; private set; } = new List<string>();
        public Dictionary<string, uint> PackCurrecnies { get; private set; } = new Dictionary<string, uint>();

        public CBSLootbox(CatalogItem item)
        {
            bool tagExist = item.Tags != null && item.Tags.Count != 0;

            ID = item.ItemId;
            DisplayName = item.DisplayName;
            Description = item.Description;
            Category = tagExist ? item.Tags[0] : CBSConstants.UndefinedCategory;
            ExternalIconURL = item.ItemImageUrl;
            Prices = item.VirtualCurrencyPrices;
            ItemClass = item.ItemClass;
            CustomData = item.CustomData;

            RandomItemsIDs = item.Container.ResultTableContents;
            PackCurrecnies = item.Container.VirtualCurrencyContents;

            var baseData = GetCustomData<CBSItemData>();
            Type = baseData == null ? ItemType.LOOT_BOXES : baseData.ItemType;
        }
    }
}
