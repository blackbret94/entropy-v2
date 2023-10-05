using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Vashta.Entropy.PhotonExtensions;

namespace Vashta.Entropy.UI.MatchBrowser
{
    public class MatchBrowserSelector : MonoBehaviour
    {
        public GameObject MatchSelectorPrefab;
        public GameObject InflationRoot;
        public RoomListCache RoomListCache;

        private MatchBrowserSelectorUnit _selectedUnit;

        private void Start()
        {
            RoomListCache.onUpdatedCache += Inflate;
        }
        
        private void Inflate()
        {
            // Clean up 
            Clear();
            
            // Get cache
            Dictionary<string, RoomInfo> roomList = RoomListCache.RoomList;

            int validRooms = 0;
            
            // Create new lobby rows
            foreach (KeyValuePair<string,RoomInfo> kvp in roomList)
            {
                RoomInfo room = kvp.Value;
                
                if(!room.IsVisible)
                    continue;

                // Instantiate new row
                GameObject matchSelectorRow = Instantiate(MatchSelectorPrefab, InflationRoot.transform);
                if (matchSelectorRow)
                {
                    MatchBrowserSelectorUnit unit = matchSelectorRow.GetComponent<MatchBrowserSelectorUnit>();

                    if (unit)
                    {
                        // Init row
                        unit.InitUnit(room);
                        validRooms++;
                    }
                }
            }
            
            // Show text if no rooms
            if (validRooms < 1)
            {
                // Instantiate new row
                GameObject matchSelectorRow = Instantiate(MatchSelectorPrefab, InflationRoot.transform);
                MatchBrowserSelectorUnit unit = matchSelectorRow.GetComponent<MatchBrowserSelectorUnit>();

                if (unit)
                {
                    // Init row
                    unit.SetNoRoomsFound();
                }
            }
        }

        private void Clear()
        {
            foreach (Transform child in InflationRoot.transform) {
                Destroy(child.gameObject);
            }
        }

        public void Select(MatchBrowserSelectorUnit selectedUnit)
        {
            _selectedUnit = selectedUnit;
        }
    }
}