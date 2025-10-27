// Decompiled with JetBrains decompiler
// Type: BodyGuards.BodyGuards
// Assembly: BodyGuards, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B87C90FB-9C51-4EFD-ADF9-BA5DB7D5C29A
// Assembly location: D:\SteamLibrary\steamapps\common\Grand Theft Auto V\scripts\BodyGuards.dll

using GTA;
using GTA.Math;
using GTA.Native;
using iFruitAddon2;
using NativeUI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BodyGuards
{
    public class BodyGuards : Script
    {
        private ScriptSettings config = ScriptSettings.Load("scripts\\BodyGuards\\BodyGuards.ini");
        private int GameTimeRef = 0;
        private bool firstLoad = false;
        private int timeOut = 0;
        private int timeOut2 = 0;
        private List<Ped> BodyGuardsList = new List<Ped>();
        private List<Ped> BodyGuardsListToLoad = new List<Ped>();
        private bool speaking = false;
        private int speechTimer = 0;
        private bool arrange = false;
        private bool greeting = false;
        private int guardNumber = 1;
        private bool numberSelected = false;
        private bool allInVehicle = false;
        private int i = 0;
        private int price = ScriptSettings.Load("scripts\\BodyGuards\\BodyGuards.ini").GetValue<int>("SETTINGS", "PRICE", 5000);
        private bool notified = false;
        private bool payed = false;
        private bool waitForPayment = false;
        private int waitForPaymentTimer = 0;
        private int noteTimer = 0;
        private bool followAutoOrder = true;
        private bool shortFormation = false;
        private bool inMenu = false;
        private List<UIMenu> mainMenuListUI = new List<UIMenu>();
        private List<object> mainMenuListString = new List<object>();
        private Color btnColor1 = Color.Empty;
        private Color btnColor2 = Color.White;
        private bool disbandProcess = false;
        private int disbandTimer = 0;
        private CustomiFruit _iFruit;
        private iFruitContact contactA;
        private Vehicle guardsVehicle;
        private int playersGroup;
        private MenuPool modMenuPool;
        private UIMenu mainMenu;
        private bool Wandering = false;
        private bool EnRoute = false;
        private bool Rushing = false;
        private Blip blip;
        private Vehicle LastCar;

        public BodyGuards()
        {
            if (Game.IsPaused)
                return;
            this.Tick += new EventHandler(this.OnTick);
            this._iFruit = new CustomiFruit();
            this.contactA = new iFruitContact("Call bodyguards");
            this.contactA.Answered += new ContactAnsweredEvent(this.ContactAnsweredDate);
            this.contactA.DialTimeout = 3;
            this.contactA.Active = true;
            this.contactA.Icon = ContactIcon.MP_Merryweather;
            this.contactA.Bold = false;
            this._iFruit.Contacts.Add(this.contactA);
        }

        private int GetHashKey(string value)
        {
            return Function.Call<int>(Hash._0xD24D37CC275948CC, (InputArgument)value);
        }

        private void groupFormationFunc(int group, int formation)
        {
            Function.Call(Hash._0xCE2F5FC3AF7E8C1E, (InputArgument)group, (InputArgument)formation);
        }

        private void groupDistanceFunc(int group, float x, float y, float z)
        {
            Function.Call(Hash._0x1D9D45004C28C916, (InputArgument)group, (InputArgument)x, (InputArgument)y, (InputArgument)z);
        }

        private void addToGroup(Ped guard)
        {
            if (Function.Call<bool>(Hash._0x5891CAC5D4ACFF74, (InputArgument)guard, (InputArgument)this.playersGroup))
                return;
            Function.Call(Hash._0x9F3480FE65DB31B5, (InputArgument)guard, (InputArgument)this.playersGroup);
        }

        private void removeFromGroup(Ped guard)
        {
            if (!Function.Call<bool>(Hash._0x5891CAC5D4ACFF74, (InputArgument)guard, (InputArgument)this.playersGroup))
                return;
            Function.Call(Hash._0xED74007FFB146BC2, (InputArgument)guard, (InputArgument)this.playersGroup);
        }

        private void groupPositive()
        {
            foreach (Ped bodyGuards in this.BodyGuardsList)
                bodyGuards.Task.ClearAll();
            Function.Call(Hash._0x8E04FEDD28D42462, (InputArgument)this.BodyGuardsList.ElementAt<Ped>(0), (InputArgument)"GENERIC_YES", (InputArgument)"SPEECH_PARAMS_FORCE", (InputArgument)1);
        }

        private void onMenuClose(UIMenu sender)
        {
            this.mainMenuListString.Clear();
            this.ClosePhoneFunc(0, 100);
            this.inMenu = false;
        }

        private void ContactAnsweredDate(iFruitContact contact)
        {
            this._iFruit.Close();
            if (Game.Player.Character.Model == (Model)PedHash.Michael)
                Function.Call(Hash._0xA4E8E696C532FBC7, (InputArgument)0);
            if (Game.Player.Character.Model == (Model)PedHash.Franklin)
                Function.Call(Hash._0xA4E8E696C532FBC7, (InputArgument)2);
            if (Game.Player.Character.Model == (Model)PedHash.Trevor)
                Function.Call(Hash._0xA4E8E696C532FBC7, (InputArgument)1);
            if (Game.Player.Character.Model != (Model)PedHash.Michael && Game.Player.Character.Model != (Model)PedHash.Franklin && Game.Player.Character.Model != (Model)PedHash.Trevor)
                Function.Call(Hash._0xA4E8E696C532FBC7, (InputArgument)0);
            if (this.BodyGuardsList.Count<Ped>() > 0)
                this.inMenu = true;
            else if (!this.waitForPayment)
            {
                this.notified = false;
                this.price = 5000;
                this.guardNumber = 1;
                this.speaking = true;
                this.speechTimer = 0;
                this.arrange = true;
                this.payed = false;
            }
            else
            {
                if (this.payed)
                    return;
                if (Game.Player.Money >= this.price / 100 * 5)
                {
                    this.payed = true;
                    Function.Call(Hash._0x67C540AA08E4A6F5, (InputArgument) (- 1), (InputArgument)"OTHER_TEXT", (InputArgument)"HUD_AWARDS");
                    this.notify("Thank you for choosing our ~y~Company");
                    this.notify("We send bodyguards to your current ~g~Location");
                    Function.Call(Hash._0x202709F4C58A0424, (InputArgument)"STRING");
                    Function.Call(Hash._0x6C188BE134E074AA, (InputArgument)"");
                    Function.Call(Hash._0x17430B918701C342, (InputArgument)0, (InputArgument)0, (InputArgument)0, (InputArgument)200);
                    Function.Call(Hash._0x531B84E7DA981FB6, (InputArgument)"CHAR_BANK_MAZE", (InputArgument)"CHAR_BANK_MAZE", (InputArgument)true, (InputArgument)7, (InputArgument)"~b~MAZE:", (InputArgument)"Payment transaction ~o~5%~w~ is written off", (InputArgument)1f, (InputArgument)"", (InputArgument)4);
                    Function.Call(Hash._0x2ED7843F8F801023, (InputArgument)true, (InputArgument)true);
                    Game.Player.Money -= this.price + this.price / 100 * 5;
                    Script.Wait(3000);
                    if (!Game.IsScreenFadedOut && !Game.IsScreenFadingOut)
                    {
                        this.ClosePhoneFunc(0, 0);
                        Game.Player.Character.Task.ClearSecondary();
                        this.speechTimer = 0;
                        this.speaking = false;
                        this.arrange = false;
                        UI.Notify("Backup is on the way!", true);
                        Game.PlaySound("PICK_UP_WEAPON", "HUD_FRONTEND_CUSTOM_SOUNDSET");
                        Script.Wait(2000);
                    }
                    this.guardsVehicleCreateFunc();
                    for (this.i = 0; this.i < this.guardNumber; ++this.i)
                        this.guardsCreateFunc(PedHash.Blackops01SMY);
                    this.greeting = true;
                    foreach (Ped bodyGuards in this.BodyGuardsList)
                    {
                        if ((Entity)bodyGuards != (Entity)null && bodyGuards.Exists() && bodyGuards.IsAlive && !bodyGuards.IsInVehicle(this.guardsVehicle) && (Entity)this.guardsVehicle != (Entity)null && this.guardsVehicle.Exists())
                        {
                            if (this.guardsVehicle.IsSeatFree(VehicleSeat.Driver))
                                bodyGuards.SetIntoVehicle(this.guardsVehicle, VehicleSeat.Driver);
                            else
                                bodyGuards.SetIntoVehicle(this.guardsVehicle, VehicleSeat.Any);
                        }
                    }
                    this.waitForPayment = false;
                    this.payed = false;
                }
                else
                {
                    this.payed = false;
                    this.waitForPayment = false;
                    this.clearScriptFunction();
                    Function.Call(Hash._0x67C540AA08E4A6F5, (InputArgument)(-1), (InputArgument)"OTHER_TEXT", (InputArgument)"HUD_AWARDS");
                    Function.Call(Hash._0x202709F4C58A0424, (InputArgument)"STRING");
                    Function.Call(Hash._0x6C188BE134E074AA, (InputArgument)"");
                    Function.Call(Hash._0x17430B918701C342, (InputArgument)0, (InputArgument)0, (InputArgument)0, (InputArgument)200);
                    Function.Call(Hash._0x531B84E7DA981FB6, (InputArgument)"CHAR_BANK_MAZE", (InputArgument)"CHAR_BANK_MAZE", (InputArgument)true, (InputArgument)7, (InputArgument)"~b~MAZE:", (InputArgument)"Payment transaction ~r~Failed", (InputArgument)1f, (InputArgument)"", (InputArgument)4);
                    Function.Call(Hash._0x2ED7843F8F801023, (InputArgument)true, (InputArgument)true);
                    Function.Call(Hash._0x202709F4C58A0424, (InputArgument)"STRING");
                    Function.Call(Hash._0x6C188BE134E074AA, (InputArgument)"");
                    Function.Call(Hash._0x17430B918701C342, (InputArgument)0, (InputArgument)0, (InputArgument)0, (InputArgument)200);
                    Function.Call(Hash._0x531B84E7DA981FB6, (InputArgument)"CHAR_BANK_MAZE", (InputArgument)"CHAR_BANK_MAZE", (InputArgument)true, (InputArgument)7, (InputArgument)"~b~MAZE:", (InputArgument)"Not enough money on your Bank ~g~Account", (InputArgument)1f, (InputArgument)"", (InputArgument)4);
                    Function.Call(Hash._0x2ED7843F8F801023, (InputArgument)true, (InputArgument)true);
                }
            }
        }

        private void guardsCreateFunc(PedHash GuardModel)
        {
            Vector3 safeCoordForPed = World.GetSafeCoordForPed(new Vector3(Game.Player.Character.Position.X + 150f, Game.Player.Character.Position.Y + 150f, Game.Player.Character.Position.Z));
            Ped ped = World.CreatePed((Model)GuardModel, safeCoordForPed);
            PedHash[] values = (PedHash[])Enum.GetValues(typeof(PedHash));
            if (!((Entity)ped != (Entity)null) || !ped.Exists() || !ped.IsAlive)
                return;
            ped.RandomizeOutfit();
            ped.Weapons.Give(WeaponHash.SMG, 999, false, false);
            if (!this.BodyGuardsList.Contains(ped))
                this.BodyGuardsList.Add(ped);
            if (!ped.CurrentBlip.Exists())
                ped.AddBlip();
            if (ped.CurrentBlip.Exists())
            {
                ped.CurrentBlip.Sprite = BlipSprite.Armor;
                ped.CurrentBlip.Name = "BodyGuard" + this.BodyGuardsList.IndexOf(ped).ToString();
                ped.CurrentBlip.Color = BlipColor.BlueDark;
                ped.CurrentBlip.IsFlashing = false;
                ped.CurrentBlip.Alpha = 200;
            }
            this.playersGroup = Function.Call<int>(Hash._0xF162E133B4E7A675, (InputArgument)Game.Player.Character);
        }

        private void guardsVehicleCreateFunc()
        {
            VehicleHash vehicleHash = VehicleHash.Baller2;
            bool flag = false;
            Vehicle[] allVehicles = World.GetAllVehicles((Model)vehicleHash);
            if (allVehicles == null || allVehicles.Length == 0)
            {
                Vector3 position = new Vector3(Game.Player.Character.Position.X + 150f, Game.Player.Character.Position.Y + 150f, Game.Player.Character.Position.Z);
                World.GetSafeCoordForPed(position);
                this.guardsVehicle = World.CreateVehicle((Model)vehicleHash, position);
                if (!((Entity)this.guardsVehicle != (Entity)null) || !this.guardsVehicle.Exists())
                    return;
                this.guardsVehicle.LockStatus = VehicleLockStatus.Unlocked;
                this.guardsVehicle.IsPersistent = true;
                this.guardsVehicle.IsBulletProof = true;
                this.guardsVehicle.PrimaryColor = VehicleColor.MetallicBlack;
                this.guardsVehicle.SecondaryColor = VehicleColor.MetallicBlack;
                this.guardsVehicle.InstallModKit();
                this.guardsVehicle.PlaceOnGround();
                this.guardsVehicle.PlaceOnNextStreet();
            }
            else
            {
                foreach (Vehicle vehicle in allVehicles)
                {
                    if ((Entity)vehicle != (Entity)null && vehicle.Exists() && (Entity)vehicle != (Entity)null && vehicle.Exists() && vehicle.PrimaryColor == VehicleColor.MetallicBlack && vehicle.SecondaryColor == VehicleColor.MetallicBlack && vehicle.IsBulletProof && (double)vehicle.Position.DistanceTo(Game.Player.Character.Position) < 50.0)
                    {
                        this.guardsVehicle = vehicle;
                        this.guardsVehicle.IsPersistent = true;
                        this.guardsVehicle.LockStatus = VehicleLockStatus.Unlocked;
                        flag = true;
                        break;
                    }
                }
                if (!flag && (Entity)this.guardsVehicle == (Entity)null)
                {
                    Vector3 position = new Vector3(Game.Player.Character.Position.X + 150f, Game.Player.Character.Position.Y + 150f, Game.Player.Character.Position.Z);
                    World.GetSafeCoordForPed(position);
                    this.guardsVehicle = World.CreateVehicle((Model)vehicleHash, position);
                    if ((Entity)this.guardsVehicle != (Entity)null && this.guardsVehicle.Exists())
                    {
                        this.guardsVehicle.LockStatus = VehicleLockStatus.Unlocked;
                        this.guardsVehicle.IsPersistent = true;
                        this.guardsVehicle.IsBulletProof = true;
                        this.guardsVehicle.PrimaryColor = VehicleColor.MetallicBlack;
                        this.guardsVehicle.SecondaryColor = VehicleColor.MetallicBlack;
                        this.guardsVehicle.InstallModKit();
                        this.guardsVehicle.PlaceOnGround();
                        this.guardsVehicle.PlaceOnNextStreet();
                    }
                }
            }
        }

        private void followInCar()
        {
            Vehicle lastVehicle = Game.Player.LastVehicle;
            if (!Game.Player.Character.IsInVehicle(lastVehicle))
                return;
            foreach (Ped bodyGuards in this.BodyGuardsList)
            {
                if ((Entity)lastVehicle != (Entity)null && lastVehicle.Exists() && lastVehicle.IsDriveable)
                {
                    if (this.BodyGuardsList.IndexOf(bodyGuards) == 0)
                    {
                        if ((!bodyGuards.IsInVehicle(lastVehicle) || bodyGuards.SeatIndex != VehicleSeat.Driver) && this.timeOut <= 0)
                        {
                            if (!Function.Call<bool>(Hash._0xBB062B2B5722478E, (InputArgument)bodyGuards))
                            {
                                bodyGuards.Task.EnterVehicle(lastVehicle, VehicleSeat.Driver, -1, 10f);
                                this.timeOut = 300;
                            }
                        }
                    }
                    else if (this.BodyGuardsList.ElementAt<Ped>(0).IsInVehicle(lastVehicle))
                    {
                        if (bodyGuards.IsSittingInVehicle(lastVehicle))
                        {
                            this.allInVehicle = true;
                        }
                        else
                        {
                            if (!Function.Call<bool>(Hash._0xBB062B2B5722478E, (InputArgument)bodyGuards))
                            {
                                if (this.timeOut2 > 0)
                                    --this.timeOut2;
                                if (this.BodyGuardsList.IndexOf(bodyGuards) == 1 && this.timeOut2 <= 0)
                                {
                                    bodyGuards.Task.EnterVehicle(this.BodyGuardsList.ElementAt<Ped>(0).CurrentVehicle, VehicleSeat.Passenger, -1, 5f);
                                    this.timeOut2 = 100;
                                }
                                if (this.BodyGuardsList.IndexOf(bodyGuards) == 2 && this.timeOut2 <= 0)
                                {
                                    bodyGuards.Task.EnterVehicle(this.BodyGuardsList.ElementAt<Ped>(0).CurrentVehicle, VehicleSeat.LeftRear, -1, 5f);
                                    this.timeOut2 = 100;
                                }
                                if (this.BodyGuardsList.IndexOf(bodyGuards) == 3 && this.timeOut2 <= 0)
                                {
                                    bodyGuards.Task.EnterVehicle(this.BodyGuardsList.ElementAt<Ped>(0).CurrentVehicle, VehicleSeat.RightRear, -1, 5f);
                                    this.timeOut2 = 100;
                                }
                            }
                            this.allInVehicle = false;
                        }
                    }
                }
            }
        }

        private void clearScriptFunction()
        {
            foreach (Ped bodyGuards in this.BodyGuardsList)
            {
                if (bodyGuards.Exists() && bodyGuards.IsAlive)
                {
                    if (Function.Call<bool>(Hash._0x5891CAC5D4ACFF74, (InputArgument)bodyGuards, (InputArgument)this.playersGroup))
                        Function.Call(Hash._0xED74007FFB146BC2, (InputArgument)bodyGuards, (InputArgument)this.playersGroup);
                    bodyGuards.Task.FleeFrom(Game.Player.Character, -1);
                }
                if (bodyGuards.CurrentBlip.Exists())
                    bodyGuards.CurrentBlip.Remove();
            }
            if ((Entity)this.guardsVehicle != (Entity)null)
            {
                if (this.guardsVehicle.Exists())
                    this.guardsVehicle.MarkAsNoLongerNeeded();
                this.guardsVehicle = (Vehicle)null;
            }
            this.mainMenuListString.Clear();
            this.disbandProcess = false;
            this.disbandTimer = 100;
            this.inMenu = false;
            this.arrange = false;
            this.allInVehicle = false;
            this.notified = false;
            this.i = 0;
            this.guardNumber = 1;
            this.numberSelected = false;
            this.greeting = false;
            this.speaking = false;
            this.speechTimer = 0;
            this.price = 5000;
            this.ClosePhoneFunc(0, 0);
            this.BodyGuardsList.Clear();
            this.followAutoOrder = true;
        }

        private void ClosePhoneFunc(int ms, int ifruitCloseMS)
        {
            Script.Wait(ms);
            this._iFruit.Close(ifruitCloseMS);
            Function.Call(Hash._0x3BC861DF703E5097);
            Game.Player.Character.Task.ClearSecondary();
        }

        private void notify(string text)
        {
            Function.Call(Hash._0x67C540AA08E4A6F5, (InputArgument)(-1), (InputArgument)"OTHER_TEXT", (InputArgument)"HUD_AWARDS");
            Function.Call(Hash._0x202709F4C58A0424, (InputArgument)"STRING");
            Function.Call(Hash._0x6C188BE134E074AA, (InputArgument)"");
            Function.Call(Hash._0x17430B918701C342, (InputArgument)(int)byte.MaxValue, (InputArgument)0, (InputArgument)0, (InputArgument)200);
            Function.Call(Hash._0x531B84E7DA981FB6, (InputArgument)"CHAR_MP_MERRYWEATHER", (InputArgument)"CHAR_MP_MERRYWEATHER", (InputArgument)true, (InputArgument)7, (InputArgument)"BODYGUARD SERVICE", (InputArgument)text, (InputArgument)1f, (InputArgument)"", (InputArgument)4);
            Function.Call(Hash._0x2ED7843F8F801023, (InputArgument)true, (InputArgument)true);
        }

        private void notify2(string text, string subject)
        {
            Function.Call(Hash._0x67C540AA08E4A6F5, (InputArgument)(-1), (InputArgument)"OTHER_TEXT", (InputArgument)"HUD_AWARDS");
            Function.Call(Hash._0x202709F4C58A0424, (InputArgument)"STRING");
            Function.Call(Hash._0x6C188BE134E074AA, (InputArgument)"");
            Function.Call(Hash._0x17430B918701C342, (InputArgument)(int)byte.MaxValue, (InputArgument)0, (InputArgument)0, (InputArgument)200);
            Function.Call(Hash._0x531B84E7DA981FB6, (InputArgument)"CHAR_MP_MERRYWEATHER", (InputArgument)"CHAR_MP_MERRYWEATHER", (InputArgument)true, (InputArgument)7, (InputArgument)text, (InputArgument)subject, (InputArgument)1f, (InputArgument)"", (InputArgument)4);
            Function.Call(Hash._0x2ED7843F8F801023, (InputArgument)true, (InputArgument)true);
        }

        public void Draw(int w, int x, int y, int z)
        {
            Scaleform scaleform = new Scaleform(0);
            scaleform.Load("instructional_buttons");
            scaleform.CallFunction("CLEAR_ALL");
            scaleform.CallFunction("TOGGLE_MOUSE_BUTTONS", (object)0);
            scaleform.CallFunction("CREATE_CONTAINER");
            scaleform.CallFunction("SET_DATA_SLOT", (object)0, (object)Function.Call<string>(Hash._0x0499D7B09FC9B407, (InputArgument)2, (InputArgument)y), (object)"Cancel");
            scaleform.CallFunction("SET_DATA_SLOT", (object)1, (object)Function.Call<string>(Hash._0x0499D7B09FC9B407, (InputArgument)2, (InputArgument)x), (object)"Accept");
            scaleform.CallFunction("SET_DATA_SLOT", (object)2, (object)Function.Call<string>(Hash._0x0499D7B09FC9B407, (InputArgument)2, (InputArgument)z), (object)"Add Guard");
            scaleform.CallFunction("SET_DATA_SLOT", (object)3, (object)Function.Call<string>(Hash._0x0499D7B09FC9B407, (InputArgument)2, (InputArgument)w), (object)"Remove Guard");
            scaleform.CallFunction("DRAW_INSTRUCTIONAL_BUTTONS", (object)-1);
            scaleform.Render2D();
        }

        public void Draw2(int y, int x)
        {
            Scaleform scaleform = new Scaleform(1);
            scaleform.Load("instructional_buttons");
            scaleform.CallFunction("CLEAR_ALL");
            scaleform.CallFunction("TOGGLE_MOUSE_BUTTONS", (object)0);
            scaleform.CallFunction("CREATE_CONTAINER");
            scaleform.CallFunction("SET_DATA_SLOT", (object)0, (object)Function.Call<string>(Hash._0x0499D7B09FC9B407, (InputArgument)2, (InputArgument)y), (object)"Cancel");
            scaleform.CallFunction("SET_DATA_SLOT", (object)1, (object)Function.Call<string>(Hash._0x0499D7B09FC9B407, (InputArgument)2, (InputArgument)x), (object)"Accept");
            scaleform.CallFunction("DRAW_INSTRUCTIONAL_BUTTONS", (object)-1);
            scaleform.Render2D();
        }

        public void Draw3(int x, int y, int w, int s, int a, int d, int q, int r)
        {
            Scaleform scaleform = new Scaleform(2);
            scaleform.Load("instructional_buttons");
            scaleform.CallFunction("CLEAR_ALL");
            scaleform.CallFunction("TOGGLE_MOUSE_BUTTONS", (object)0);
            scaleform.CallFunction("CREATE_CONTAINER");
            scaleform.CallFunction("SET_DATA_SLOT", (object)0, (object)Function.Call<string>(Hash._0x0499D7B09FC9B407, (InputArgument)2, (InputArgument)y), (object)"Cancel");
            scaleform.CallFunction("SET_DATA_SLOT", (object)1, (object)Function.Call<string>(Hash._0x0499D7B09FC9B407, (InputArgument)2, (InputArgument)x), (object)"Weapons");
            scaleform.CallFunction("SET_DATA_SLOT", (object)2, (object)Function.Call<string>(Hash._0x0499D7B09FC9B407, (InputArgument)2, (InputArgument)a), (object)"Default");
            scaleform.CallFunction("SET_DATA_SLOT", (object)3, (object)Function.Call<string>(Hash._0x0499D7B09FC9B407, (InputArgument)2, (InputArgument)w), (object)"Circle");
            scaleform.CallFunction("SET_DATA_SLOT", (object)4, (object)Function.Call<string>(Hash._0x0499D7B09FC9B407, (InputArgument)2, (InputArgument)d), (object)"Line");
            scaleform.CallFunction("SET_DATA_SLOT", (object)5, (object)Function.Call<string>(Hash._0x0499D7B09FC9B407, (InputArgument)2, (InputArgument)s), (object)"Space");
            scaleform.CallFunction("SET_DATA_SLOT", (object)6, (object)Function.Call<string>(Hash._0x0499D7B09FC9B407, (InputArgument)2, (InputArgument)q), (object)"Disband");
            scaleform.CallFunction("SET_DATA_SLOT", (object)7, (object)Function.Call<string>(Hash._0x0499D7B09FC9B407, (InputArgument)2, (InputArgument)r), (object)"Move/stay");
            scaleform.CallFunction("DRAW_INSTRUCTIONAL_BUTTONS", (object)-1);
            scaleform.Render2D();
        }

        public void Draw4(int v, int z, int y, int h, int q)
        {
            Scaleform scaleform = new Scaleform(3);
            scaleform.Load("instructional_buttons");
            scaleform.CallFunction("CLEAR_ALL");
            scaleform.CallFunction("TOGGLE_MOUSE_BUTTONS", (object)0);
            scaleform.CallFunction("CREATE_CONTAINER");
            scaleform.CallFunction("SET_DATA_SLOT", (object)0, (object)Function.Call<string>(Hash._0x0499D7B09FC9B407, (InputArgument)2, (InputArgument)v), (object)"Drive to Waypoint");
            scaleform.CallFunction("SET_DATA_SLOT", (object)1, (object)Function.Call<string>(Hash._0x0499D7B09FC9B407, (InputArgument)2, (InputArgument)z), (object)"Drive around");
            scaleform.CallFunction("SET_DATA_SLOT", (object)2, (object)Function.Call<string>(Hash._0x0499D7B09FC9B407, (InputArgument)2, (InputArgument)y), (object)"Cancel");
            scaleform.CallFunction("SET_DATA_SLOT", (object)3, (object)Function.Call<string>(Hash._0x0499D7B09FC9B407, (InputArgument)2, (InputArgument)h), (object)"Stop");
            scaleform.CallFunction("SET_DATA_SLOT", (object)4, (object)Function.Call<string>(Hash._0x0499D7B09FC9B407, (InputArgument)2, (InputArgument)q), (object)"Rush");
            scaleform.CallFunction("DRAW_INSTRUCTIONAL_BUTTONS", (object)-1);
            scaleform.Render2D();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
        }

        private void DisableControlsFunc(bool block_movements)
        {
            Game.DisableControlThisFrame(0, GTA.Control.NextCamera);
            if (block_movements)
            {
                Game.DisableControlThisFrame(2, GTA.Control.MoveLeft);
                Game.DisableControlThisFrame(2, GTA.Control.MoveLeftOnly);
                Game.DisableControlThisFrame(2, GTA.Control.MoveRight);
                Game.DisableControlThisFrame(2, GTA.Control.MoveRightOnly);
                Game.DisableControlThisFrame(2, GTA.Control.MoveUp);
                Game.DisableControlThisFrame(2, GTA.Control.MoveUpOnly);
                Game.DisableControlThisFrame(2, GTA.Control.MoveDown);
                Game.DisableControlThisFrame(2, GTA.Control.MoveDownOnly);
                Game.DisableControlThisFrame(2, GTA.Control.ScriptPadLeft);
                Game.DisableControlThisFrame(2, GTA.Control.ScriptPadRight);
                Game.DisableControlThisFrame(2, GTA.Control.ScriptPadUp);
                Game.DisableControlThisFrame(2, GTA.Control.ScriptPadDown);
                Game.DisableControlThisFrame(2, GTA.Control.VehicleMoveLeft);
                Game.DisableControlThisFrame(2, GTA.Control.VehicleMoveRight);
                Game.DisableControlThisFrame(2, GTA.Control.VehicleMoveLeft);
                Game.DisableControlThisFrame(2, GTA.Control.VehicleMoveRightOnly);
                Game.DisableControlThisFrame(2, GTA.Control.VehicleMoveLeftOnly);
                Game.DisableControlThisFrame(2, GTA.Control.VehicleMoveLeftRight);
            }
            Game.DisableControlThisFrame(2, GTA.Control.VehicleLookBehind);
            Game.DisableControlThisFrame(2, GTA.Control.LookBehind);
            Game.DisableControlThisFrame(2, GTA.Control.NextCamera);
            Game.DisableControlThisFrame(2, GTA.Control.VehicleCinCam);
            Game.DisableControlThisFrame(2, GTA.Control.VehicleNextRadio);
            Game.DisableControlThisFrame(2, GTA.Control.VehiclePrevRadio);
            Game.DisableControlThisFrame(2, GTA.Control.VehicleHeadlight);
            Game.DisableControlThisFrame(2, GTA.Control.VehicleHorn);
            Game.DisableControlThisFrame(2, GTA.Control.VehicleHandbrake);
            Game.DisableControlThisFrame(2, GTA.Control.VehicleExit);
            Game.DisableControlThisFrame(2, GTA.Control.SelectWeapon);
            Game.DisableControlThisFrame(2, GTA.Control.Phone);
            Game.DisableControlThisFrame(2, GTA.Control.PhoneCameraDOF);
            Game.DisableControlThisFrame(2, GTA.Control.PhoneCameraExpression);
            Game.DisableControlThisFrame(2, GTA.Control.PhoneCameraFocusLock);
            Game.DisableControlThisFrame(2, GTA.Control.PhoneCameraGrid);
            Game.DisableControlThisFrame(2, GTA.Control.PhoneCameraSelfie);
            Game.DisableControlThisFrame(2, GTA.Control.PhoneCancel);
            Game.DisableControlThisFrame(2, GTA.Control.PhoneDown);
            Game.DisableControlThisFrame(2, GTA.Control.PhoneLeft);
            Game.DisableControlThisFrame(2, GTA.Control.PhoneRight);
            Game.DisableControlThisFrame(2, GTA.Control.PhoneUp);
            Game.DisableControlThisFrame(2, GTA.Control.PhoneOption);
            Game.DisableControlThisFrame(2, GTA.Control.PhoneExtraOption);
            Game.DisableControlThisFrame(2, GTA.Control.PhoneSelect);
            Game.DisableControlThisFrame(2, GTA.Control.Aim);
            Game.DisableControlThisFrame(2, GTA.Control.Attack);
            Game.DisableControlThisFrame(2, GTA.Control.Attack2);
            Game.DisableControlThisFrame(2, GTA.Control.Sprint);
            Game.DisableControlThisFrame(2, GTA.Control.Reload);
            Game.DisableControlThisFrame(2, GTA.Control.Phone);
            Game.DisableControlThisFrame(2, GTA.Control.Jump);
            Game.DisableControlThisFrame(2, GTA.Control.VehicleRocketBoost);
            Game.DisableControlThisFrame(2, GTA.Control.RappelJump);
            Game.DisableControlThisFrame(2, GTA.Control.RappelLongJump);
            Game.DisableControlThisFrame(2, GTA.Control.VehicleJump);
            Game.DisableControlThisFrame(2, GTA.Control.Cover);
            Game.DisableControlThisFrame(2, GTA.Control.MeleeAttack1);
            Game.DisableControlThisFrame(2, GTA.Control.MeleeAttack2);
            Game.DisableControlThisFrame(2, GTA.Control.MeleeAttackAlternate);
            Game.DisableControlThisFrame(2, GTA.Control.MeleeAttackHeavy);
            Game.DisableControlThisFrame(2, GTA.Control.MeleeAttackLight);
            Game.DisableControlThisFrame(2, GTA.Control.Context);
            Game.DisableControlThisFrame(2, GTA.Control.ContextSecondary);
            Game.DisableControlThisFrame(2, GTA.Control.AccurateAim);
            Game.DisableControlThisFrame(2, GTA.Control.Duck);
            Game.DisableControlThisFrame(2, GTA.Control.Enter);
            Game.DisableControlThisFrame(2, GTA.Control.VehicleAccelerate);
        }

        private void UsePhoneFunc(Ped ped)
        {
            this.DisableControlsFunc(false);
            Function.Call(Hash._0xC4E2813898C97A4B, (InputArgument)false);
            if (Function.Call<bool>(Hash._0x2AFE52F782F25775, (InputArgument)(Entity)ped))
                return;
            if (ped.Model == (Model)PedHash.Michael)
                Function.Call(Hash._0xA4E8E696C532FBC7, (InputArgument)0);
            if (ped.Model == (Model)PedHash.Franklin)
                Function.Call(Hash._0xA4E8E696C532FBC7, (InputArgument)2);
            if (ped.Model == (Model)PedHash.Trevor)
                Function.Call(Hash._0xA4E8E696C532FBC7, (InputArgument)1);
            if (!(ped.Model != (Model)PedHash.Michael) || !(ped.Model != (Model)PedHash.Franklin) || !(ped.Model != (Model)PedHash.Trevor))
                return;
            Function.Call(Hash._0xA4E8E696C532FBC7, (InputArgument)0);
        }

        private void EndRoute()
        {
            if (!this.EnRoute && !this.Wandering || !((Entity)this.LastCar.Driver != (Entity)null) || this.LastCar.IsSeatFree(VehicleSeat.Driver))
                return;
            if (Game.Player.Character.IsInVehicle(this.LastCar))
            {
                this.blip = Function.Call<Blip>(Hash._0x1BEDE233E6CD2A1F, (InputArgument)8);
                if (!this.blip.Exists() && this.EnRoute)
                {
                    foreach (Ped bodyGuards in this.BodyGuardsList)
                    {
                        this.LastCar.Driver.DrivingSpeed = 0.0f;
                        Script.Wait(7000);
                        this.LastCar.Driver.Task.ClearAll();
                        this.EnRoute = false;
                    }
                }
            }
            else
            {
                foreach (Ped bodyGuards in this.BodyGuardsList)
                {
                    this.EnRoute = false;
                    this.Wandering = false;
                    this.LastCar.Driver.DrivingSpeed = 0.0f;
                    Script.Wait(7000);
                    this.LastCar.Driver.Task.ClearAll();
                    this.LastCar.Driver.Task.LeaveVehicle();
                }
            }
        }

        private void OnTick(object sender, EventArgs e)
        {
            this._iFruit.Update();
            if (this.BodyGuardsList.Count<Ped>() > 0)
            {
                this.contactA.Name = "Control Bodyguards";
                this.contactA.Active = true;
                this.contactA.DialTimeout = 10;
            }
            else if (!this.waitForPayment)
            {
                this.contactA.Name = "Call Bodyguards";
                this.contactA.Active = true;
                this.contactA.DialTimeout = 10;
            }
            else
            {
                this.contactA.Name = "Pay for Service";
                this.contactA.Active = true;
                this.contactA.DialTimeout = 10;
            }
            if (!this.firstLoad)
                this.firstLoad = true;
            if (this.speaking)
            {
                if (!Function.Call<bool>(Hash._0x2AFE52F782F25775, (InputArgument)Game.Player.Character))
                    this.UsePhoneFunc(Game.Player.Character);
                if (Function.Call<bool>(Hash._0x2AFE52F782F25775, (InputArgument)Game.Player.Character))
                {
                    Function.Call(Hash._0xC4E2813898C97A4B, (InputArgument)false);
                    Function.Call(Hash._0x015C49A93E3E086E, (InputArgument)true);
                    this.UsePhoneFunc(Game.Player.Character);
                    if (this.arrange)
                    {
                        int x = 176;
                        int y = 177;
                        int z = 172;
                        int w = 173;
                        if (this.numberSelected)
                        {
                            this.Draw2(y, x);
                            ++this.noteTimer;
                            if (this.noteTimer >= 100)
                            {
                                Function.Call(Hash._0x202709F4C58A0424, (InputArgument)"STRING");
                                Function.Call(Hash._0x6C188BE134E074AA, (InputArgument)"");
                                Function.Call(Hash._0x17430B918701C342, (InputArgument)(int)byte.MaxValue, (InputArgument)0, (InputArgument)0, (InputArgument)200);
                                Function.Call(Hash._0x531B84E7DA981FB6, (InputArgument)"CHAR_MP_MERRYWEATHER", (InputArgument)"CHAR_MP_MERRYWEATHER", (InputArgument)true, (InputArgument)7, (InputArgument)"BODYGUARD SERVICE", (InputArgument)("Number of Guards: ~g~" + this.guardNumber.ToString() + "~n~~w~ Cost: " + this.price.ToString() + " ~g~$"), (InputArgument)1f, (InputArgument)"", (InputArgument)4);
                                Function.Call(Hash._0x2ED7843F8F801023, (InputArgument)true, (InputArgument)true);
                                this.noteTimer = 0;
                            }
                            if (Function.Call<bool>(Hash._0x91AEF906BCA88877, (InputArgument)0, (InputArgument)y))
                            {
                                Function.Call(Hash._0x95C9E72F3D7DEC9B, (InputArgument)1);
                                this.speaking = false;
                                this.speechTimer = 0;
                                this.clearScriptFunction();
                            }
                            if (Function.Call<bool>(Hash._0x91AEF906BCA88877, (InputArgument)0, (InputArgument)x))
                            {
                                Function.Call(Hash._0x67C540AA08E4A6F5, (InputArgument)(-1), (InputArgument)"OTHER_TEXT", (InputArgument)"HUD_AWARDS");
                                Function.Call(Hash._0x95C9E72F3D7DEC9B, (InputArgument)4);
                                this.waitForPaymentTimer = 0;
                                this.waitForPayment = true;
                                this.speaking = false;
                                this.speechTimer = 0;
                            }
                        }
                        else
                        {
                            DisplayHelpTextThisFrame("PRICE: " + this.price.ToString() + " ~g~$ ~w~~n~GUARDS ~y~: " + this.guardNumber.ToString() + " / ~y~4");
                            this.Draw(w, x, y, z);
                            if (!this.notified)
                                this.notified = true;
                            if (Function.Call<bool>(Hash._0x91AEF906BCA88877, (InputArgument)0, (InputArgument)x))
                            {
                                Function.Call(Hash._0x67C540AA08E4A6F5, (InputArgument)(-1), (InputArgument)"OTHER_TEXT", (InputArgument)"HUD_AWARDS");
                                Function.Call(Hash._0x95C9E72F3D7DEC9B, (InputArgument)4);
                                this.numberSelected = true;
                                this.waitForPayment = false;
                                this.waitForPaymentTimer = 0;
                                this.noteTimer = 70;
                                Script.Wait(1500);
                            }
                            if (Function.Call<bool>(Hash._0x91AEF906BCA88877, (InputArgument)0, (InputArgument)z))
                            {
                                Function.Call(Hash._0x95C9E72F3D7DEC9B, (InputArgument)3);
                                Function.Call(Hash._0x67C540AA08E4A6F5, (InputArgument)(-1), (InputArgument)"OTHER_TEXT", (InputArgument)"HUD_AWARDS");
                                if (this.guardNumber < 4)
                                {
                                    ++this.guardNumber;
                                    this.price *= 2;
                                }
                                else
                                    UI.Notify("Number of guards is ~y~Maximal", true);
                            }
                            if (Function.Call<bool>(Hash._0x91AEF906BCA88877, (InputArgument)0, (InputArgument)y))
                            {
                                Function.Call(Hash._0x95C9E72F3D7DEC9B, (InputArgument)1);
                                Function.Call(Hash._0x67C540AA08E4A6F5, (InputArgument)(-1), (InputArgument)"OTHER_TEXT", (InputArgument)"HUD_AWARDS");
                                this.speaking = false;
                                this.speechTimer = 0;
                                this.clearScriptFunction();
                            }
                            if (Function.Call<bool>(Hash._0x91AEF906BCA88877, (InputArgument)0, (InputArgument)w))
                            {
                                Function.Call(Hash._0x95C9E72F3D7DEC9B, (InputArgument)3);
                                Function.Call(Hash._0x67C540AA08E4A6F5, (InputArgument)(-1), (InputArgument)"OTHER_TEXT", (InputArgument)"HUD_AWARDS");
                                if (this.guardNumber > 1)
                                {
                                    --this.guardNumber;
                                    this.price /= 2;
                                }
                                else
                                    UI.Notify("Number of guards is ~y~Minimal", true);
                            }
                        }

                         void DisplayHelpTextThisFrame(string text)
                        {
                            Function.Call(Hash._0x8509B634FBE7DA11, (InputArgument)"STRING");
                            Function.Call(Hash._0x6C188BE134E074AA, (InputArgument)text);
                            Function.Call(Hash._0x238FFE5C7B0498A6, (InputArgument)0, (InputArgument)0, (InputArgument)1, (InputArgument)(-1));
                        }
                    }
                    else
                    {
                        ++this.speechTimer;
                        if (this.speechTimer == 250)
                        {
                            DisplayHelpTextThisFrame("~BLIP_INFO_ICON~ The ~y~Deal ~w~has been ~r~canceled");
                            this.clearScriptFunction();
                            this.ClosePhoneFunc(0, 0);
                        }

                         void DisplayHelpTextThisFrame(string text)
                        {
                            Function.Call(Hash._0x8509B634FBE7DA11, (InputArgument)"STRING");
                            Function.Call(Hash._0x6C188BE134E074AA, (InputArgument)text);
                            Function.Call(Hash._0x238FFE5C7B0498A6, (InputArgument)0, (InputArgument)0, (InputArgument)1, (InputArgument)(-1));
                        }
                    }
                }
            }
            if (this.waitForPayment)
            {
                if (this.waitForPaymentTimer < 2000)
                {
                    ++this.noteTimer;
                    if (this.noteTimer >= 100)
                    {
                        Function.Call(Hash._0x202709F4C58A0424, (InputArgument)"STRING");
                        Function.Call(Hash._0x6C188BE134E074AA, (InputArgument)"");
                        Function.Call(Hash._0x17430B918701C342, (InputArgument)(int)byte.MaxValue, (InputArgument)0, (InputArgument)0, (InputArgument)200);
                        Function.Call(Hash._0x531B84E7DA981FB6, (InputArgument)"CHAR_MP_MERRYWEATHER", (InputArgument)"CHAR_MP_MERRYWEATHER", (InputArgument)true, (InputArgument)7, (InputArgument)"BODYGUARD SERVICE", (InputArgument)("Number of Guards: ~g~" + this.guardNumber.ToString() + "~n~~w~ Cost: " + this.price.ToString() + " ~g~$"), (InputArgument)1f, (InputArgument)"", (InputArgument)4);
                        Function.Call(Hash._0x2ED7843F8F801023, (InputArgument)true, (InputArgument)true);
                        this.noteTimer = 0;
                    }
                    DisplayHelpTextThisFrame("~BLIP_INFO_ICON~ Pay for ~g~Bodyguard ~w~service with your ~y~Phone");
                }
                else
                {
                    this.waitForPayment = false;
                    this.payed = false;
                    this.clearScriptFunction();
                }

                 void DisplayHelpTextThisFrame(string text)
                {
                    Function.Call(Hash._0x8509B634FBE7DA11, (InputArgument)"STRING");
                    Function.Call(Hash._0x6C188BE134E074AA, (InputArgument)text);
                    Function.Call(Hash._0x238FFE5C7B0498A6, (InputArgument)0, (InputArgument)0, (InputArgument)1, (InputArgument)(-1));
                }
            }
            if (this.BodyGuardsList != null && this.BodyGuardsList.Count<Ped>() > 0 && !this.greeting && this.followAutoOrder)
            {
                if (this.timeOut > 0)
                    --this.timeOut;
                int num;
                if (!Game.Player.Character.IsInVehicle())
                    num = Function.Call<bool>(Hash._0xBB062B2B5722478E, (InputArgument)Game.Player.Character) ? 1 : 0;
                else
                    num = 1;
                if (num != 0)
                {
                    this.followInCar();
                }
                else
                {
                    foreach (Ped bodyGuards in this.BodyGuardsList)
                    {
                        if (bodyGuards.IsSittingInVehicle())
                        {
                            if ((double)Game.Player.Character.Position.DistanceTo(bodyGuards.Position) < 17.0 && bodyGuards.CurrentVehicle.IsStopped)
                            {
                                if (!Function.Call<bool>(Hash._0x5891CAC5D4ACFF74, (InputArgument)bodyGuards, (InputArgument)this.playersGroup))
                                    this.addToGroup(bodyGuards);
                            }
                        }
                        else
                            this.addToGroup(bodyGuards);
                    }
                }
            }
            if (this.BodyGuardsList != null && this.BodyGuardsList.Count<Ped>() > 0 && this.greeting)
            {
                int num = Game.Player.Character.IsInCombat ? 0 : ((double)Game.Player.Character.Position.DistanceTo(this.BodyGuardsList.ElementAt<Ped>(0).Position) < 15.0 ? 1 : 0);
                if ((Entity)this.guardsVehicle != (Entity)null)
                {
                    if ((double)Game.Player.Character.Position.DistanceTo(this.guardsVehicle.Position) > 17.0)
                    {
                        if ((Entity)this.guardsVehicle.Driver != (Entity)null)
                        {
                            if (this.timeOut > 0)
                                --this.timeOut;
                            if (this.timeOut <= 0)
                            {
                                this.guardsVehicle.Driver.Task.DriveTo(this.guardsVehicle, Game.Player.Character.Position, 17f, 40f, 2883621);
                                this.timeOut = 200;
                            }
                        }
                    }
                    else
                    {
                        Ped driver = this.guardsVehicle.Driver;
                        if (driver.IsInVehicle() && (Entity)driver != (Entity)null)
                        {
                            foreach (Ped passenger in this.guardsVehicle.Passengers)
                            {
                                if (passenger.IsSittingInVehicle(this.guardsVehicle))
                                {
                                    passenger.Task.LeaveVehicle();
                                    Script.Wait(1000);
                                    passenger.Task.TurnTo((Entity)Game.Player.Character);
                                }
                            }
                            if (driver.IsSittingInVehicle(this.guardsVehicle))
                                driver.Task.LeaveVehicle();
                        }
                        else
                        {
                            foreach (Ped bodyGuards in this.BodyGuardsList)
                            {
                                if (this.timeOut > 0)
                                    --this.timeOut;
                                if (this.BodyGuardsList.IndexOf(bodyGuards) == 0)
                                {
                                    if ((double)bodyGuards.Position.DistanceTo(Game.Player.Character.Position) > 3.5)
                                    {
                                        if (this.timeOut <= 0)
                                        {
                                            bodyGuards.Task.GoTo(Game.Player.Character.Position);
                                            this.timeOut = 100;
                                        }
                                    }
                                    else if (this.timeOut <= 0)
                                    {
                                        if (Function.Call<bool>(Hash._0xD71649DB0A545AA3, (InputArgument)bodyGuards, (InputArgument)Game.Player.Character, (InputArgument)10f))
                                        {
                                            bodyGuards.Task.ClearAll();
                                            Function.Call(Hash._0x8E04FEDD28D42462, (InputArgument)bodyGuards, (InputArgument)"GENERIC_HI", (InputArgument)"SPEECH_PARAMS_FORCE", (InputArgument)1);
                                            this.greeting = false;
                                        }
                                        else
                                            bodyGuards.Task.TurnTo((Entity)Game.Player.Character);
                                        this.timeOut = 100;
                                    }
                                }
                                else if (!Function.Call<bool>(Hash._0xD71649DB0A545AA3, (InputArgument)bodyGuards, (InputArgument)Game.Player.Character, (InputArgument)10f))
                                    bodyGuards.Task.TurnTo((Entity)Game.Player.Character, 300);
                                else
                                    bodyGuards.Task.ClearAll();
                            }
                        }
                    }
                }
                else
                    this.greeting = false;
            }
            if (this.disbandProcess)
            {
                if (this.disbandTimer > 0)
                    --this.disbandTimer;
                else if (this.modMenuPool == null)
                {
                    this.clearScriptFunction();
                    this.disbandProcess = false;
                }
            }
            if (this.GameTimeRef < Game.GameTime)
            {
                this.GameTimeRef = Game.GameTime + 1000;
                if (!this.speaking)
                {
                    foreach (Ped bodyGuards in this.BodyGuardsList)
                    {
                        if ((Entity)bodyGuards != (Entity)null && (!bodyGuards.Exists() || bodyGuards.IsDead) && bodyGuards.CurrentBlip.Exists())
                            bodyGuards.CurrentBlip.Remove();
                    }
                }
                if (Game.Player.Character.IsDead)
                    this.clearScriptFunction();
            }
            if (this.inMenu && Game.Player.Character.IsOnFoot)
            {
                Function.Call(Hash._0xC4E2813898C97A4B, (InputArgument)false);
                Function.Call(Hash._0x015C49A93E3E086E, (InputArgument)true);
                this.UsePhoneFunc(Game.Player.Character);
                int x = 176;
                int y = 177;
                int w = 172;
                int a = 174;
                int d = 175;
                int s = 173;
                int q = 179;
                int r = 178;
                this.Draw3(x, y, w, s, a, d, q, r);
                if (Function.Call<bool>(Hash._0x91AEF906BCA88877, (InputArgument)0, (InputArgument)r))
                {
                    Function.Call(Hash._0x95C9E72F3D7DEC9B, (InputArgument)4);
                    this.groupPositive();
                    if (this.followAutoOrder)
                    {
                        foreach (Ped bodyGuards in this.BodyGuardsList)
                        {
                            this.notify2("Group:", "Movement: ~o~Guard");
                            this.followAutoOrder = false;
                            this.removeFromGroup(bodyGuards);
                            bodyGuards.CanSwitchWeapons = false;
                            Weapon bestWeapon = bodyGuards.Weapons.BestWeapon;
                            bodyGuards.Weapons.Select(bestWeapon.Hash, true);
                            bodyGuards.Task.GuardCurrentPosition();
                        }
                    }
                    else
                    {
                        foreach (Ped bodyGuards in this.BodyGuardsList)
                        {
                            this.notify2("Group:", "Movement: ~g~Follow");
                            this.followAutoOrder = true;
                            this.addToGroup(bodyGuards);
                        }
                    }
                }
                if (Function.Call<bool>(Hash._0x91AEF906BCA88877, (InputArgument)0, (InputArgument)q))
                {
                    this.notify("Hope you enjoyed our ~y~Service~w~,sir");
                    Function.Call(Hash._0x95C9E72F3D7DEC9B, (InputArgument)1);
                    this.inMenu = false;
                    this.ClosePhoneFunc(0, 1000);
                    Function.Call(Hash._0xB138AAB8A70D3C69, (InputArgument)"TREVOR_SMALL_01");
                    this.groupPositive();
                    this.disbandProcess = true;
                    this.disbandTimer = 100;
                }
                if (Function.Call<bool>(Hash._0x91AEF906BCA88877, (InputArgument)0, (InputArgument)y))
                {
                    Function.Call(Hash._0x95C9E72F3D7DEC9B, (InputArgument)2);
                    this.inMenu = false;
                    this.ClosePhoneFunc(0, 1000);
                }
                if (Function.Call<bool>(Hash._0x91AEF906BCA88877, (InputArgument)0, (InputArgument)x))
                {
                    Function.Call(Hash._0x95C9E72F3D7DEC9B, (InputArgument)3);
                    this.groupPositive();
                    if (this.BodyGuardsList.ElementAt<Ped>(0).Weapons.Current.Group == WeaponGroup.Unarmed)
                    {
                        foreach (Ped bodyGuards in this.BodyGuardsList)
                        {
                            bodyGuards.CanSwitchWeapons = false;
                            Weapon bestWeapon = bodyGuards.Weapons.BestWeapon;
                            bodyGuards.Weapons.Select(bestWeapon.Hash, true);
                            this.notify2("Group:", "Weapons: ~o~Free");
                        }
                    }
                    else
                    {
                        foreach (Ped bodyGuards in this.BodyGuardsList)
                        {
                            this.notify2("Group:", "Weapons: ~g~Holsted");
                            bodyGuards.CanSwitchWeapons = true;
                            bodyGuards.Weapons.Select(WeaponHash.Unarmed, true);
                        }
                    }
                }
                if (Function.Call<bool>(Hash._0x91AEF906BCA88877, (InputArgument)0, (InputArgument)a))
                {
                    this.notify2("Group:", "Formation: ~o~Free");
                    Function.Call(Hash._0x95C9E72F3D7DEC9B, (InputArgument)1);
                    this.groupPositive();
                    this.groupFormationFunc(Function.Call<int>(Hash._0xF162E133B4E7A675, (InputArgument)this.BodyGuardsList.ElementAt<Ped>(0)), 0);
                }
                if (Function.Call<bool>(Hash._0x91AEF906BCA88877, (InputArgument)0, (InputArgument)w))
                {
                    this.notify2("Group:", "Formation: ~y~Circle");
                    Function.Call(Hash._0x95C9E72F3D7DEC9B, (InputArgument)2);
                    this.groupPositive();
                    this.groupFormationFunc(Function.Call<int>(Hash._0xF162E133B4E7A675, (InputArgument)this.BodyGuardsList.ElementAt<Ped>(0)), 1);
                }
                if (Function.Call<bool>(Hash._0x91AEF906BCA88877, (InputArgument)0, (InputArgument)d))
                {
                    this.notify2("Group:", "Formation: ~g~Line");
                    Function.Call(Hash._0x95C9E72F3D7DEC9B, (InputArgument)3);
                    this.groupPositive();
                    this.groupFormationFunc(Function.Call<int>(Hash._0xF162E133B4E7A675, (InputArgument)this.BodyGuardsList.ElementAt<Ped>(0)), 3);
                }
                if (Function.Call<bool>(Hash._0x91AEF906BCA88877, (InputArgument)0, (InputArgument)s))
                {
                    Function.Call(Hash._0x95C9E72F3D7DEC9B, (InputArgument)4);
                    this.groupPositive();
                    int group = Function.Call<int>(Hash._0xF162E133B4E7A675, (InputArgument)this.BodyGuardsList.ElementAt<Ped>(0));
                    if (this.shortFormation)
                    {
                        this.notify2("Group:", "Space range: ~o~Long");
                        this.groupDistanceFunc(group, 5.5f, 5.5f, 5.5f);
                        this.shortFormation = !this.shortFormation;
                    }
                    else
                    {
                        this.notify2("Group:", "Space range: ~g~Short");
                        this.groupDistanceFunc(group, 2.5f, 2.5f, 2.5f);
                        this.shortFormation = !this.shortFormation;
                    }
                }
            }
            else if (this.inMenu && Game.Player.Character.IsInVehicle())
            {
                Function.Call(Hash._0xC4E2813898C97A4B, (InputArgument)false);
                Function.Call(Hash._0x015C49A93E3E086E, (InputArgument)true);
                this.UsePhoneFunc(Game.Player.Character);
                int v = 51;
                int z = 72;
                int y = 177;
                int h = 179;
                int q = 171;
                this.Draw4(v, z, y, h, q);
                if (Function.Call<bool>(Hash._0x91AEF906BCA88877, (InputArgument)0, (InputArgument)z))
                {
                    Function.Call(Hash._0x95C9E72F3D7DEC9B, (InputArgument)1);
                    this.LastCar = Game.Player.LastVehicle;
                    if (!this.LastCar.IsSeatFree(VehicleSeat.Driver) && this.LastCar.Driver.IsSittingInVehicle(this.LastCar))
                    {
                        if (!this.Wandering)
                        {
                            this.LastCar.Driver.Task.ClearAll();
                            Function.Call(Hash._0x480142959D337D00, (InputArgument)this.LastCar.Driver, (InputArgument)this.LastCar, (InputArgument)35f, (InputArgument)786603);
                            this.Wandering = true;
                            UI.Notify("Driving", true);
                        }
                        else
                        {
                            this.LastCar.Driver.DrivingSpeed = 0.0f;
                            Script.Wait(3000);
                            this.LastCar.Driver.Task.ClearAll();
                            this.Wandering = false;
                            UI.Notify("Cancelling", true);
                        }
                    }
                    else
                    {
                        foreach (Ped bodyGuards in this.BodyGuardsList)
                        {
                            bodyGuards.Task.ClearAll();
                            bodyGuards.Task.EnterVehicle(this.LastCar, VehicleSeat.Driver, 6000, 10f);
                            UI.Notify("Driver not found.", true);
                        }
                    }
                }
                if (Function.Call<bool>(Hash._0x91AEF906BCA88877, (InputArgument)0, (InputArgument)h))
                {
                    Function.Call(Hash._0x95C9E72F3D7DEC9B, (InputArgument)3);
                    UI.Notify("Stopping.", true);
                    this.LastCar.Driver.DrivingSpeed = 0.0f;
                    Script.Wait(7000);
                    this.LastCar.Driver.Task.ClearAll();
                    this.Wandering = false;
                    this.EnRoute = false;
                }
                if (Function.Call<bool>(Hash._0x91AEF906BCA88877, (InputArgument)0, (InputArgument)q))
                {
                    Function.Call(Hash._0x95C9E72F3D7DEC9B, (InputArgument)4);
                    if (!this.Rushing)
                    {
                        UI.Notify("Driving Style: Avoid", true);
                        this.LastCar.Driver.DrivingStyle = DrivingStyle.Rushed;
                        this.Rushing = true;
                     
                    }
                    else
                    {
                        UI.Notify("Driving Style: Normal", true);
                        this.Rushing = false;
                        this.LastCar.Driver.DrivingStyle = DrivingStyle.Normal;
                    }
                }
                if (Function.Call<bool>(Hash._0x91AEF906BCA88877, (InputArgument)0, (InputArgument)y))
                {
                    Function.Call(Hash._0x95C9E72F3D7DEC9B, (InputArgument)2);
                    this.inMenu = false;
                    this.ClosePhoneFunc(0, 1000);
                }
                if (Function.Call<bool>(Hash._0x91AEF906BCA88877, (InputArgument)0, (InputArgument)v))
                {
                    Function.Call(Hash._0x95C9E72F3D7DEC9B, (InputArgument)2);
                    this.blip = Function.Call<Blip>(Hash._0x1BEDE233E6CD2A1F, (InputArgument)8);
                    this.LastCar = Game.Player.LastVehicle;
                    Ped driver = this.LastCar.Driver;
                    Ped character = Game.Player.Character;
                    if ((Entity)driver == (Entity)character)
                        return;
                    foreach (Ped bodyGuards in this.BodyGuardsList)
                    {
                        if (this.blip.Exists() && Game.Player.Character.IsInVehicle(this.LastCar))
                        {
                            if (!this.LastCar.IsSeatFree(VehicleSeat.Driver))
                            {
                                bodyGuards.Task.ClearAll();
                                this.LastCar.Driver.Task.ClearAll();
                                this.LastCar.Driver.Task.DriveTo(this.LastCar, World.GetWaypointPosition(), 10f, 40f, 786603);
                                UI.Notify("Driving to waypoint.", true);
                                this.EnRoute = true;
                                break;
                            }
                            bodyGuards.Task.ClearAll();
                            bodyGuards.Task.EnterVehicle(this.LastCar, VehicleSeat.Driver, 6000, 10f);
                            UI.Notify("Driver not in vehicle.", true);
                            break;
                        }
                        this.LastCar.Speed = 0.0f;
                        Script.Wait(6000);
                        driver.Task.ClearAll();
                        UI.Notify("Waypoint not set.", true);
                    }
                }
            }
            this.EndRoute();
        }
    }
}
