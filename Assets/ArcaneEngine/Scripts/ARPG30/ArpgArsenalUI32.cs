using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArcaneEngine
{
    public enum ArpgArsenalTab32 { Inventory, Stash, Currency, Maps, Cores, Runes, Crafting, Vendor, LootFilter, Statistics }

    public sealed class ArpgArsenalUI32 : MonoBehaviour
    {
        public static ArpgArsenalUI32 Instance { get; private set; }

        private bool _open;
        private ArpgArsenalTab32 _tab;
        private string _selectedItemId, _selectedMapId, _selectedStashTab="stash.general.1", _dragItemId, _search=string.Empty, _message=string.Empty;
        private Vector2 _dragOffset, _scroll;
        private float _messageUntil;
        private GUIStyle _title,_heading,_small,_wrap;
        private Texture2D _backdrop,_panel,_cell;
        private string _lastCharacter;
        private int _lastMapCount;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap()
        {
            if(Instance!=null)return;
            GameObject host=new GameObject("Arcane Engine 3.2 · Arsenal & Economy");
            DontDestroyOnLoad(host);host.AddComponent<ArpgArsenalUI32>();
        }

        private void Awake()
        {
            if(Instance!=null&&Instance!=this){Destroy(gameObject);return;}
            Instance=this;DontDestroyOnLoad(gameObject);ArpgArsenalContent32.Ensure();
        }

        private void OnDestroy()
        {
            if(Instance==this)Instance=null;
            if(_backdrop!=null)Destroy(_backdrop);if(_panel!=null)Destroy(_panel);if(_cell!=null)Destroy(_cell);
        }

        private void Update()
        {
            ArpgProfile30 p=ArpgFoundation30.Profile;
            if(p!=null&&p.characterId!=_lastCharacter)
            {
                _lastCharacter=p.characterId;ArpgCharacterArsenal32 c=ArpgArsenalStore32.EnsureProfile(p);
                _selectedStashTab=c==null?"stash.general.1":c.selectedStashTabId;_lastMapCount=p.totalMapsCompleted;ArpgVendor32.Ensure(p);
            }
            if(p!=null&&p.totalMapsCompleted!=_lastMapCount){_lastMapCount=p.totalMapsCompleted;ArpgVendor32.Ensure(p);}
            if(_open&&ArpgInput31.CancelPressed())Close();
        }

        public void Open(ArpgArsenalTab32 tab)
        {
            if(ArpgFoundation30.Profile==null)return;
            if(ArpgInterface30.Instance!=null&&ArpgInterface30.Instance.IsOpen)ArpgInterface30.Instance.ClosePanel();
            _tab=tab;_open=true;Cursor.visible=true;Cursor.lockState=CursorLockMode.None;
        }
        public void Close(){_open=false;_dragItemId=null;Cursor.visible=false;Cursor.lockState=CursorLockMode.Locked;}

        private void OnGUI()
        {
            if(ArpgFrontend31.Instance==null||!ArpgFrontend31.Instance.IsGameplay)return;
            ArpgProfile30 p=ArpgFoundation30.Profile;if(p==null)return;EnsureStyles();

            if(!_open)
            {
                if(GUI.Button(new Rect(18f,Screen.height-66f,230f,44f),"ARSENAL & ECONOMY"))Open(ArpgArsenalTab32.Inventory);
                return;
            }

            GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),_backdrop);
            Rect w=new Rect(28f,24f,Screen.width-56f,Screen.height-48f);GUI.DrawTexture(w,_panel);
            GUI.Label(new Rect(w.x+22,w.y+12,650,44),"ARSENAL & ECONOMY · 3.2.0",_title);
            GUI.Label(new Rect(w.x+690,w.y+20,420,28),p.characterName+" · "+ArpgArsenalStore32.Current.gold+" Gold",_heading);
            if(GUI.Button(new Rect(w.xMax-112,w.y+14,90,34),"CLOSE"))Close();
            DrawTabs(new Rect(w.x+20,w.y+64,w.width-40,38));
            Rect body=new Rect(w.x+20,w.y+112,w.width-40,w.height-138);
            switch(_tab)
            {
                case ArpgArsenalTab32.Inventory:DrawInventory(p,body);break;
                case ArpgArsenalTab32.Stash:DrawStash(p,body);break;
                case ArpgArsenalTab32.Currency:DrawCurrency(body);break;
                case ArpgArsenalTab32.Maps:DrawMaps(p,body);break;
                case ArpgArsenalTab32.Cores:DrawCollection(p.ownedCoreIds,"SPELL CORES",body);break;
                case ArpgArsenalTab32.Runes:DrawCollection(p.ownedRuneIds,"SUPPORT RUNES",body);break;
                case ArpgArsenalTab32.Crafting:DrawCrafting(p,body);break;
                case ArpgArsenalTab32.Vendor:DrawVendor(p,body);break;
                case ArpgArsenalTab32.LootFilter:DrawFilter(body);break;
                case ArpgArsenalTab32.Statistics:DrawStats(p,body);break;
            }
            if(!string.IsNullOrEmpty(_message)&&Time.unscaledTime<_messageUntil)GUI.Label(new Rect(w.x+260,w.yMax-46,w.width-520,32),_message,GUI.skin.box);
        }

        private void DrawTabs(Rect r)
        {
            string[] names={"Inventory","Stash","Currency","Maps","Cores","Runes","Crafting","Vendor","Loot Filter","Statistics"};
            float width=r.width/names.Length;
            for(int i=0;i<names.Length;i++)if(GUI.Toggle(new Rect(r.x+i*width,r.y,width-3,r.height),(int)_tab==i,names[i],GUI.skin.button))_tab=(ArpgArsenalTab32)i;
        }

        private void DrawInventory(ArpgProfile30 p,Rect body)
        {
            Rect equipment=new Rect(body.x,body.y,260,body.height);Rect grid=new Rect(equipment.xMax+16,body.y+34,Mathf.Min(720,body.width*.52f),300);
            Rect detail=new Rect(grid.xMax+16,body.y,body.xMax-grid.xMax-16,body.height);
            GUI.Box(equipment,GUIContent.none);GUI.Label(new Rect(equipment.x+14,equipment.y+12,equipment.width-28,30),"EQUIPPED",_heading);
            float y=equipment.y+48;
            foreach(ArpgItemSlot30 slot in Enum.GetValues(typeof(ArpgItemSlot30)).Cast<ArpgItemSlot30>())
            {
                ArpgItem30 item=p.Equipped(slot);
                if(GUI.Button(new Rect(equipment.x+12,y,equipment.width-24,36),slot+": "+(item==null?"Empty":item.displayName)))
                { _selectedItemId=item==null?string.Empty:item.instanceId;if(item!=null&&Event.current.button==1)Unequip(p,item); }
                y+=39;
            }
            GUI.Label(new Rect(grid.x,body.y,grid.width,28),"BACKPACK · 12 × 5",_heading);
            DrawGrid(p,ArpgArsenalStore32.InventoryContainer(p.characterId),12,5,grid);
            DrawSelected(p,detail,false);
        }

        private void DrawStash(ArpgProfile30 p,Rect body)
        {
            List<ArpgStashTab32> tabs=ArpgArsenalStore32.Current.stashTabs.OrderBy(x=>x.order).ToList();
            Rect side=new Rect(body.x,body.y,190,body.height);GUI.Box(side,GUIContent.none);GUI.Label(new Rect(side.x+12,side.y+10,side.width-24,28),"SHARED STASH",_heading);
            float y=side.y+46;
            foreach(ArpgStashTab32 t in tabs){if(GUI.Toggle(new Rect(side.x+10,y,side.width-20,34),_selectedStashTab==t.id,t.displayName,GUI.skin.button))_selectedStashTab=t.id;y+=37;}
            ArpgStashTab32 tab=ArpgArsenalStore32.StashTab(_selectedStashTab);Rect content=new Rect(side.xMax+16,body.y,body.width-side.width-16,body.height);
            if(tab==null)return;
            if(tab.kind==ArpgStashTabKind32.Currency){DrawCurrency(content);return;}if(tab.kind==ArpgStashTabKind32.Maps){DrawMaps(p,content);return;}
            if(tab.kind==ArpgStashTabKind32.Cores){DrawCollection(p.ownedCoreIds,"SPELL CORES",content);return;}if(tab.kind==ArpgStashTabKind32.Runes){DrawCollection(p.ownedRuneIds,"SUPPORT RUNES",content);return;}
            GUI.Label(new Rect(content.x,content.y,520,28),tab.displayName+" · "+tab.columns+" × "+tab.rows,_heading);
            GUI.SetNextControlName("ArsenalSearch");_search=GUI.TextField(new Rect(content.x+540,content.y,280,30),_search??string.Empty);
            Rect grid=new Rect(content.x,content.y+40,Mathf.Min(720,content.width*.62f),content.height-50);
            DrawGrid(p,tab.id,tab.columns,tab.rows,grid);
            DrawSelected(p,new Rect(grid.xMax+16,content.y+40,content.xMax-grid.xMax-16,content.height-50),true);
        }

        private void DrawGrid(ArpgProfile30 p,string container,int cols,int rows,Rect rect)
        {
            float cell=Mathf.Min(rect.width/cols,rect.height/rows);Rect actual=new Rect(rect.x,rect.y,cell*cols,cell*rows);GUI.Box(actual,GUIContent.none);
            for(int y=0;y<rows;y++)for(int x=0;x<cols;x++)GUI.DrawTexture(new Rect(actual.x+x*cell+1,actual.y+y*cell+1,cell-2,cell-2),_cell);
            foreach(ArpgItemState32 state in ArpgArsenalStore32.InContainer(container).ToList())
            {
                ArpgItem30 item=FindItem(p,state.itemInstanceId);if(item==null)continue;
                if(!string.IsNullOrEmpty(_search)&&item.displayName.IndexOf(_search,StringComparison.OrdinalIgnoreCase)<0&&ArpgItems30.Describe(item).IndexOf(_search,StringComparison.OrdinalIgnoreCase)<0)continue;
                Rect ir=new Rect(actual.x+state.x*cell+2,actual.y+state.y*cell+2,state.width*cell-4,state.height*cell-4);
                DrawItem(item,state,ir);HandleItem(p,item,state,ir,container);
            }
            Event e=Event.current;
            if(!string.IsNullOrEmpty(_dragItemId)&&e.type==EventType.MouseUp&&actual.Contains(e.mousePosition))
            {
                ArpgItem30 item=FindItem(p,_dragItemId);ArpgItemState32 state=ArpgArsenalStore32.ItemState(_dragItemId);
                if(item!=null&&state!=null)
                {
                    int x=Mathf.FloorToInt((e.mousePosition.x-actual.x-_dragOffset.x)/cell),y=Mathf.FloorToInt((e.mousePosition.y-actual.y-_dragOffset.y)/cell);
                    string result;ArpgArsenalStore32.Move(p,item,container,x,y,out result);SetMessage(result);
                }
                _dragItemId=null;e.Use();
            }
            if(!string.IsNullOrEmpty(_dragItemId))
            {
                ArpgItem30 item=FindItem(p,_dragItemId);ArpgItemState32 state=ArpgArsenalStore32.ItemState(_dragItemId);
                if(item!=null&&state!=null)
                {
                    Rect ghost=new Rect(e.mousePosition.x-_dragOffset.x,e.mousePosition.y-_dragOffset.y,state.width*cell-4,state.height*cell-4);
                    Color c=GUI.color;GUI.color=new Color(c.r,c.g,c.b,.72f);DrawItem(item,state,ghost);GUI.color=c;
                }
            }
        }

        private void DrawItem(ArpgItem30 item,ArpgItemState32 state,Rect r)
        {
            Color c=GUI.color;GUI.color=RarityColor(item.rarity);GUI.Box(r,GUIContent.none);GUI.color=c;
            GUI.Label(new Rect(r.x+4,r.y+3,r.width-8,r.height-6),(state.favorite?"★ ":item.locked?"◆ ":"")+item.displayName,_small);
        }

        private void HandleItem(ArpgProfile30 p,ArpgItem30 item,ArpgItemState32 state,Rect r,string container)
        {
            Event e=Event.current;if(!r.Contains(e.mousePosition)||e.type!=EventType.MouseDown)return;_selectedItemId=item.instanceId;
            if(e.button==1){if(p.IsEquipped(item.instanceId))Unequip(p,item);else Equip(p,item);}
            else if(e.shift)
            {
                string target=container.StartsWith("inventory:")?"stash.general.1":ArpgArsenalStore32.InventoryContainer(p.characterId),result;
                ArpgArsenalStore32.AutoPlace(p,item,target,out result);SetMessage(result);
            }
            else{_dragItemId=item.instanceId;_dragOffset=e.mousePosition-new Vector2(r.x,r.y);}
            e.Use();
        }

        private void DrawSelected(ArpgProfile30 p,Rect r,bool stash)
        {
            GUI.Box(r,GUIContent.none);ArpgItem30 item=FindItem(p,_selectedItemId);
            if(item==null){GUI.Label(new Rect(r.x+16,r.y+16,r.width-32,34),"SELECT AN ITEM",_heading);return;}
            GUI.Label(new Rect(r.x+16,r.y+14,r.width-32,44),item.displayName,_heading);string description=ArpgItems30.Describe(item);
            _scroll=GUI.BeginScrollView(new Rect(r.x+12,r.y+62,r.width-24,r.height-240),_scroll,new Rect(0,0,r.width-54,Mathf.Max(420,_wrap.CalcHeight(new GUIContent(description),r.width-60)+20)));
            GUI.Label(new Rect(8,4,r.width-70,800),description,_wrap);GUI.EndScrollView();
            ArpgItemState32 state=ArpgArsenalStore32.ItemState(item.instanceId);float y=r.yMax-166;
            if(GUI.Button(new Rect(r.x+12,y,r.width*.47f,34),p.IsEquipped(item.instanceId)?"UNEQUIP":"EQUIP")){if(p.IsEquipped(item.instanceId))Unequip(p,item);else Equip(p,item);}
            if(GUI.Button(new Rect(r.x+r.width*.51f,y,r.width*.47f-12,34),stash?"TO BACKPACK":"TO STASH"))
            {string result;ArpgArsenalStore32.AutoPlace(p,item,stash?ArpgArsenalStore32.InventoryContainer(p.characterId):"stash.general.1",out result);SetMessage(result);}
            y+=38;
            if(GUI.Button(new Rect(r.x+12,y,r.width*.31f,34),item.locked?"UNLOCK":"LOCK")){item.locked=!item.locked;ArpgProfileStore30.Save(p);SetMessage(item.locked?"Item locked.":"Item unlocked.");}
            if(GUI.Button(new Rect(r.x+r.width*.345f,y,r.width*.31f,34),state!=null&&state.favorite?"UNFAVORITE":"FAVORITE")){state.favorite=!state.favorite;ArpgArsenalStore32.Save();SetMessage(state.favorite?"Favorited.":"Favorite removed.");}
            if(GUI.Button(new Rect(r.x+r.width*.68f,y,r.width*.3f-12,34),"SALVAGE")){string result;if(ArpgVendor32.Salvage(p,item,out result))_selectedItemId=string.Empty;SetMessage(result);}
            y+=38;
            if(GUI.Button(new Rect(r.x+12,y,r.width*.47f,34),"SELL · "+ArpgVendor32.SellValue(item)+" GOLD")){string result;if(ArpgVendor32.Sell(p,item,out result))_selectedItemId=string.Empty;SetMessage(result);}
            GUI.enabled=!item.corrupted&&!item.locked;if(GUI.Button(new Rect(r.x+r.width*.51f,y,r.width*.47f-12,34),"CORRUPT")){string result;ArpgCrafting32.Corrupt(p,item,out result);SetMessage(result);}GUI.enabled=true;
        }

        private void DrawCurrency(Rect body)
        {
            GUI.Box(body,GUIContent.none);GUI.Label(new Rect(body.x+18,body.y+14,500,34),"ACCOUNT-WIDE CURRENCY",_heading);int i=0;
            foreach(ArpgCurrencyDefinition32 d in ArpgArsenalContent32.CurrencyDefinitions)
            {
                int c=i%3,row=i/3;Rect card=new Rect(body.x+18+c*((body.width-54)/3),body.y+58+row*96,(body.width-72)/3,84);GUI.Box(card,GUIContent.none);
                GUI.Label(new Rect(card.x+10,card.y+8,card.width-20,24),d.displayName+" · "+ArpgArsenalStore32.Currency(d.id),_heading);
                GUI.Label(new Rect(card.x+10,card.y+34,card.width-20,42),d.description+" ["+d.rarity+"]",_small);i++;
            }
        }

        private void DrawMaps(ArpgProfile30 p,Rect body)
        {
            GUI.Box(body,GUIContent.none);GUI.Label(new Rect(body.x+16,body.y+12,600,32),"MAP STORAGE & CRAFTING",_heading);
            Rect list=new Rect(body.x+12,body.y+54,body.width*.42f,body.height-66),detail=new Rect(list.xMax+16,body.y+54,body.xMax-list.xMax-28,body.height-66);GUI.Box(list,GUIContent.none);
            float y=list.y+10;
            foreach(ArpgMapItem30 map in p.maps.Where(x=>x!=null).OrderBy(x=>x.tier).ThenBy(x=>x.rarity))
            {if(GUI.Toggle(new Rect(list.x+10,y,list.width-20,38),_selectedMapId==map.instanceId,"T"+map.tier+" · "+map.rarity+" · "+map.affixIds.Count+" modifiers",GUI.skin.button))_selectedMapId=map.instanceId;y+=41;if(y>list.yMax-42)break;}
            GUI.Box(detail,GUIContent.none);ArpgMapItem30 selected=p.maps.FirstOrDefault(x=>x!=null&&x.instanceId==_selectedMapId);
            if(selected==null){GUI.Label(new Rect(detail.x+16,detail.y+16,detail.width-32,30),"Select a map.",_heading);return;}
            GUI.Label(new Rect(detail.x+16,detail.y+14,detail.width-32,34),"T"+selected.tier+" "+selected.rarity+" MAP",_heading);
            float reward=(ArpgMapCrafting32.RewardMultiplier(selected)-1)*100;
            GUI.Label(new Rect(detail.x+16,detail.y+54,detail.width-32,24),"Danger +"+(selected.affixIds.Count*8)+"% · Reward +"+reward.ToString("0")+"% · Quality "+selected.quality+"%",_small);
            float ay=detail.y+86;foreach(string id in selected.affixIds){ArpgMapAffixDefinition30 a=ArpgContent30.MapAffix(id);if(a!=null){GUI.Label(new Rect(detail.x+18,ay,detail.width-36,38),"• "+a.displayName+" — "+a.description,_small);ay+=38;}}
            ArpgCurrency32[] currencies={ArpgCurrency32.FluxShard,ArpgCurrency32.SovereignEmber,ArpgCurrency32.TransferenceSigil,ArpgCurrency32.ArtisansMeasure};
            float by=detail.yMax-170;
            for(int i=0;i<currencies.Length;i++){ArpgCurrency32 c=currencies[i];ArpgCurrencyDefinition32 d=ArpgArsenalContent32.Currency(c);if(GUI.Button(new Rect(detail.x+16+(i%2)*(detail.width*.49f),by+(i/2)*40,detail.width*.47f,34),d.displayName+" · "+ArpgArsenalStore32.Currency(c))){string result;ArpgMapCrafting32.Apply(p,selected,c,out result);SetMessage(result);}}
            if(GUI.Button(new Rect(detail.x+16,detail.yMax-48,detail.width-32,34),"CORRUPT MAP")){string result;ArpgMapCrafting32.Corrupt(p,selected,out result);SetMessage(result);}
        }

        private void DrawCollection(List<string> values,string title,Rect body)
        {
            GUI.Box(body,GUIContent.none);GUI.Label(new Rect(body.x+16,body.y+14,500,32),title,_heading);GUI.SetNextControlName("ArsenalCollectionSearch");_search=GUI.TextField(new Rect(body.xMax-330,body.y+14,310,30),_search??string.Empty);
            int i=0;foreach(string value in (values??new List<string>()).Where(x=>string.IsNullOrEmpty(_search)||x.IndexOf(_search,StringComparison.OrdinalIgnoreCase)>=0).OrderBy(x=>x))
            {int c=i%4,row=i/4;Rect card=new Rect(body.x+16+c*((body.width-40)/4),body.y+60+row*58,(body.width-64)/4,50);GUI.Box(card,GUIContent.none);GUI.Label(new Rect(card.x+8,card.y+8,card.width-16,34),value,_small);i++;if(card.yMax>body.yMax-20)break;}
        }

        private void DrawCrafting(ArpgProfile30 p,Rect body)
        {
            GUI.Box(body,GUIContent.none);GUI.Label(new Rect(body.x+16,body.y+12,520,34),"CRAFTING STATION",_heading);
            Rect items=new Rect(body.x+12,body.y+54,body.width*.34f,body.height-66),ops=new Rect(items.xMax+16,body.y+54,body.xMax-items.xMax-28,body.height-66);GUI.Box(items,GUIContent.none);
            float y=items.y+8;foreach(ArpgItem30 item in p.items.Where(x=>x!=null&&!p.IsEquipped(x.instanceId)).OrderByDescending(x=>x.rarity))
            {if(GUI.Toggle(new Rect(items.x+8,y,items.width-16,38),_selectedItemId==item.instanceId,item.displayName,GUI.skin.button))_selectedItemId=item.instanceId;y+=41;if(y>items.yMax-42)break;}
            GUI.Box(ops,GUIContent.none);ArpgItem30 selected=FindItem(p,_selectedItemId);if(selected==null){GUI.Label(new Rect(ops.x+16,ops.y+16,ops.width-32,30),"Select a crafting project.",_heading);return;}
            GUI.Label(new Rect(ops.x+16,ops.y+12,ops.width-32,30),selected.displayName,_heading);GUI.Label(new Rect(ops.x+16,ops.y+44,ops.width-32,80),ArpgItems30.Describe(selected),_small);
            int i=0;foreach(ArpgCurrencyDefinition32 d in ArpgArsenalContent32.CurrencyDefinitions)
            {int c=i%3,row=i/3;Rect b=new Rect(ops.x+14+c*((ops.width-28)/3),ops.y+132+row*62,(ops.width-46)/3,54);GUI.enabled=ArpgArsenalStore32.Currency(d.id)>0&&!selected.locked;if(GUI.Button(b,d.displayName+"\n"+ArpgArsenalStore32.Currency(d.id))){string result;ArpgCrafting32.Apply(p,selected,d.id,out result);SetMessage(result);}GUI.enabled=true;i++;}
            ArpgItemState32 state=ArpgArsenalStore32.ItemState(selected.instanceId);if(state!=null&&state.craftHistory.Count>0){ArpgCraftRecord32 last=state.craftHistory[state.craftHistory.Count-1];GUI.Label(new Rect(ops.x+16,ops.yMax-72,ops.width-32,54),"Latest: "+last.operation+" — "+last.detail,_small);}
        }

        private void DrawVendor(ArpgProfile30 p,Rect body)
        {
            ArpgVendor32.Ensure(p);GUI.Box(body,GUIContent.none);GUI.Label(new Rect(body.x+16,body.y+12,600,34),"REFUGE EXCHANGE · "+ArpgArsenalStore32.Current.gold+" GOLD",_heading);
            if(GUI.Button(new Rect(body.xMax-190,body.y+12,170,32),"REFRESH STOCK")){ArpgVendor32.Refresh(p);SetMessage("Vendor refreshed.");}
            int i=0;foreach(ArpgVendorOffer32 offer in ArpgArsenalStore32.Current.vendor.offers.ToList())
            {int c=i%4,row=i/4;Rect card=new Rect(body.x+16+c*((body.width-40)/4),body.y+58+row*170,(body.width-64)/4,156);GUI.Box(card,GUIContent.none);GUI.Label(new Rect(card.x+10,card.y+8,card.width-20,38),offer.item.displayName,_heading);GUI.Label(new Rect(card.x+10,card.y+48,card.width-20,42),offer.item.rarity+" · iLvl "+offer.item.itemLevel+" · "+offer.item.slot,_small);if(GUI.Button(new Rect(card.x+10,card.yMax-46,card.width-20,34),"BUY · "+offer.goldCost)){string result;ArpgVendor32.Buy(p,offer,out result);SetMessage(result);}i++;}
            if(GUI.Button(new Rect(body.x+16,body.yMax-54,240,36),"GAMBLE MAIN HAND")){string result;ArpgVendor32.Gamble(p,ArpgItemSlot30.MainHand,out result);SetMessage(result);}
            if(GUI.Button(new Rect(body.x+266,body.yMax-54,240,36),"GAMBLE ARMOUR")){string result;ArpgVendor32.Gamble(p,ArpgItemSlot30.BodyArmour,out result);SetMessage(result);}
        }

        private void DrawFilter(Rect body)
        {
            GUI.Box(body,GUIContent.none);ArpgLootFilterPreset32 p=ArpgArsenalStore32.ActiveFilter();GUI.Label(new Rect(body.x+16,body.y+12,600,34),"LOOT FILTER · "+(p==null?"None":p.displayName),_heading);if(p==null)return;
            float y=body.y+58;foreach(ArpgLootFilterRule32 rule in p.rules)
            {Rect row=new Rect(body.x+16,y,body.width-32,54);GUI.Box(row,GUIContent.none);rule.enabled=GUI.Toggle(new Rect(row.x+10,row.y+15,24,24),rule.enabled,string.Empty);GUI.Label(new Rect(row.x+42,row.y+8,340,24),rule.displayName,_heading);GUI.Label(new Rect(row.x+42,row.y+30,500,20),rule.action+" · Minimum "+rule.minimumRarity+" · iLvl "+rule.minimumItemLevel,_small);if(GUI.Button(new Rect(row.xMax-250,row.y+10,110,34),rule.action.ToString()))rule.action=rule.action==ArpgFilterAction32.Show?ArpgFilterAction32.Hide:ArpgFilterAction32.Show;if(GUI.Button(new Rect(row.xMax-130,row.y+10,110,34),"RARITY +"))rule.minimumRarity=(ArpgItemRarity30)(((int)rule.minimumRarity+1)%5);y+=60;}
            if(GUI.Button(new Rect(body.x+16,body.yMax-48,220,34),"SAVE FILTER")){ArpgArsenalStore32.Save();SetMessage("Loot filter saved.");}
        }

        private void DrawStats(ArpgProfile30 p,Rect body)
        {
            GUI.Box(body,GUIContent.none);GUI.Label(new Rect(body.x+16,body.y+12,600,34),"ADVANCED CHARACTER STATISTICS",_heading);ArpgStatAccumulator30 stats=ArpgStatHooks30.Build(p);int i=0;
            foreach(ArpgStat30 stat in Enum.GetValues(typeof(ArpgStat30)).Cast<ArpgStat30>())
            {int c=i%3,row=i/3;Rect card=new Rect(body.x+16+c*((body.width-40)/3),body.y+56+row*42,(body.width-64)/3,36);GUI.Box(card,GUIContent.none);float v=stats.Get(stat);bool flat=stat==ArpgStat30.MaximumHealth||stat==ArpgStat30.MaximumMana||stat==ArpgStat30.Armour||stat==ArpgStat30.Evasion||stat==ArpgStat30.ArcaneWard||stat==ArpgStat30.TriggerEnergy||stat==ArpgStat30.RuneCapacity||stat==ArpgStat30.Attunement;GUI.Label(new Rect(card.x+8,card.y+8,card.width-16,20),stat+": "+(flat?v.ToString("0.##"):(v*100).ToString("0.##")+"%"),_small);i++;}
        }

        private void Equip(ArpgProfile30 p,ArpgItem30 item)
        {
            string result;if(!ArpgArsenalStore32.PrepareForCharacter(p,item,out result)){SetMessage(result);return;}if(!p.Equip(item,out result)){SetMessage(result);return;}ArpgItemState32 s=ArpgArsenalStore32.EnsureItemState(p,item);s.containerId=ArpgArsenalStore32.EquippedContainer(p.characterId,item.slot);s.ownerCharacterId=p.characterId;ArpgArsenalStore32.SyncLegacyStash(p);ArpgArsenalStore32.Save();ArpgProfileStore30.Save(p);SetMessage(result);
        }
        private void Unequip(ArpgProfile30 p,ArpgItem30 item)
        {
            if(!p.Unequip(item.slot)){SetMessage("Item is not equipped.");return;}string result;
            if(!ArpgArsenalStore32.AutoPlace(p,item,ArpgArsenalStore32.InventoryContainer(p.characterId),out result)){string restoreMessage;p.Equip(item,out restoreMessage);ArpgItemState32 s=ArpgArsenalStore32.EnsureItemState(p,item);s.containerId=ArpgArsenalStore32.EquippedContainer(p.characterId,item.slot);SetMessage(result);return;}
            ArpgProfileStore30.Save(p);SetMessage("Unequipped "+item.displayName+".");
        }
        private static ArpgItem30 FindItem(ArpgProfile30 p,string id)
        {
            if(p==null||string.IsNullOrEmpty(id))return null;ArpgItem30 item=ArpgArsenalStore32.FindItem(p,id);if(item!=null)return item;
            foreach(ArpgVendorOffer32 o in ArpgArsenalStore32.Current.vendor.offers)if(o!=null&&o.item!=null&&o.item.instanceId==id)return o.item;return null;
        }
        private void SetMessage(string v){_message=v??string.Empty;_messageUntil=Time.unscaledTime+5.5f;if(ArpgInterface30.Instance!=null)ArpgInterface30.Instance.SetMessage(_message);}
        private void EnsureStyles()
        {
            if(_title!=null)return;_title=new GUIStyle(GUI.skin.label){fontSize=25,fontStyle=FontStyle.Bold,normal={textColor=new Color(.95f,.78f,.3f)}};
            _heading=new GUIStyle(GUI.skin.label){fontSize=15,fontStyle=FontStyle.Bold,normal={textColor=Color.white}};
            _small=new GUIStyle(GUI.skin.label){fontSize=12,wordWrap=true,normal={textColor=new Color(.78f,.84f,.92f)}};
            _wrap=new GUIStyle(GUI.skin.label){fontSize=13,wordWrap=true,normal={textColor=new Color(.88f,.91f,.96f)}};
            _backdrop=Make(new Color(.015f,.022f,.04f,.96f));_panel=Make(new Color(.045f,.065f,.105f,.98f));_cell=Make(new Color(.08f,.105f,.15f,.95f));
        }
        private static Texture2D Make(Color c){Texture2D t=new Texture2D(1,1,TextureFormat.RGBA32,false);t.SetPixel(0,0,c);t.Apply();t.hideFlags=HideFlags.HideAndDontSave;return t;}
        private static Color RarityColor(ArpgItemRarity30 r){if(r==ArpgItemRarity30.Unique)return new Color(.9f,.34f,.08f);if(r==ArpgItemRarity30.Exceptional)return new Color(.66f,.22f,.95f);if(r==ArpgItemRarity30.Rare)return new Color(.88f,.68f,.16f);if(r==ArpgItemRarity30.Magic)return new Color(.25f,.48f,.9f);return new Color(.3f,.34f,.4f);}
    }
}
