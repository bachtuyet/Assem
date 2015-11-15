using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using LeagueSharp.Common.Data;
using ItemData = LeagueSharp.Common.Data.ItemData;
using FioraProject.Evade;

namespace FioraProject
{
    using static FioraPassive;
    using static GetTargets;
    using static Combos;
    public static class Program
    {
        public static Obj_AI_Hero Player { get { return ObjectManager.Player; } }

        public static Orbwalking.Orbwalker Orbwalker;

        public static Spell Q, W, E, R;

        public static Menu Menu;


        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.ChampionName != "Fiora")
                return;
            Q = new Spell(SpellSlot.Q, 400);
            W = new Spell(SpellSlot.W, 750);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R);
            W.SetSkillshot(0.75f, 80, 2000, false, SkillshotType.SkillshotLine);
            W.MinHitChance = HitChance.High;


            Menu = new Menu("Project" + Player.ChampionName, Player.ChampionName, true);
            Menu.SetFontStyle(System.Drawing.FontStyle.Bold,SharpDX.Color.DeepPink);

            Menu orbwalkerMenu = new Menu("Orbwalker", "Orbwalker");
            Orbwalker = new FioraProject.Orbwalking.Orbwalker(orbwalkerMenu);
            Menu.AddSubMenu(orbwalkerMenu);
            Menu ts = Menu.AddSubMenu(new Menu("Target Selector", "Target Selector")); ;
            TargetSelector.AddToMenu(ts);
            Menu spellMenu = Menu.AddSubMenu(new Menu("Spell", "Spell"));

            Menu Harass = spellMenu.AddSubMenu(new Menu("Rỉa máu", "Rỉa máu"));

            Menu Combo = spellMenu.AddSubMenu(new Menu("Combo", "Combo"));

            Menu Target = Menu.AddSubMenu(new Menu("Chế độ chọn mục tiêu", "Chế độ chọn mục tiêu"));

            Menu PriorityMode = Target.AddSubMenu(new Menu("Ưu tiên", "Chế độ ưu tiên"));

            Menu OptionalMode = Target.AddSubMenu(new Menu("Tùy chỉnh", "Tùy chỉnh"));

            Menu SelectedMode = Target.AddSubMenu(new Menu("Chọn", "Chế độ được chọn"));

            Menu LaneClear = spellMenu.AddSubMenu(new Menu("Đẩy đường", "Đẩy đường"));

            spellMenu.AddItem(new MenuItem("Orbwalk Last Right Click", "Orbwalk Last Right Click")
                .SetValue(new KeyBind('Y', KeyBindType.Press))).ValueChanged += OrbwalkLastClick.OrbwalkLRCLK_ValueChanged;

            Menu JungClear = spellMenu.AddSubMenu(new Menu("Fam rừng", "Fam rừng"));

            Menu Misc = Menu.AddSubMenu(new Menu("Cài đặt khác","Cài đặt khác"));

            Menu Draw = Menu.AddSubMenu(new Menu("Vòng", "Vòng")); ;

            Harass.AddItem(new MenuItem("Dùng Q rỉa máu", "Bật Q").SetValue(true));
            Harass.AddItem(new MenuItem("Dùng Q giảm khoảng cách rỉa máu", "Q để rút ngắn khoảng cách").SetValue(true));
            Harass.AddItem(new MenuItem("Dùng Q Rỉa máu Pas ", "Dùng Q trước khi thụ động").SetValue(true));
            Harass.AddItem(new MenuItem("Dùng Q Thụ động", "Sử dụng Q để đạt thụ động").SetValue(true));
            Harass.AddItem(new MenuItem("Dùng E rỉa máu", "Bật E").SetValue(true));
            Harass.AddItem(new MenuItem("Mana Rỉa máu", "Mana để rỉa máu").SetValue(new Slider(40, 0, 100)));

            Combo.AddItem(new MenuItem("Dùng Q combo", "Bật Q").SetValue(true));
            Combo.AddItem(new MenuItem("Dùng Q rút ngắn combo", "Dùng Q rút ngắn khoảng cách").SetValue(true));
            Combo.AddItem(new MenuItem("Dùng Q trước Thụ động", "Dùng Q trước khi thụ động").SetValue(true));
            Combo.AddItem(new MenuItem("Dùng Q thụ động", "Dùng Q đạt thụ động").SetValue(true));
            Combo.AddItem(new MenuItem("Rút ngắn khoảng cách với lính", "Dùng Q để rút ngắn").SetValue(false));
            Combo.AddItem(new MenuItem("Giá trị rút ngắn khoảng cách với lính", "Dùng Q rút ngắn nếu % cdr >=").SetValue(new Slider(25, 0, 40)));
            Combo.AddItem(new MenuItem("Dùng E combo", "bật E").SetValue(true));
            Combo.AddItem(new MenuItem("Dùng R Combo", "Bật R").SetValue(true));
            Combo.AddItem(new MenuItem("Dùng R ít máu", "Dùng R ít máu").SetValue(true));
            Combo.AddItem(new MenuItem("Giá trị ít máu để dùng R", "ít máu dùng R nếu player hp <").SetValue(new Slider(40, 0, 100)));
            Combo.AddItem(new MenuItem("Dùng R nếu có thể giết", "Dùng R có thể giết").SetValue(true));
            Combo.AddItem(new MenuItem(Dùng R trên tab", "Dùng trên tab").SetValue(true));
            Combo.AddItem(new MenuItem("Dùng R trên phím", "Dùng R trên phím").SetValue(new KeyBind('G', KeyBindType.Press)));
            Combo.AddItem(new MenuItem("Luôn dùng R khi Combo", "Luôn dùng R").SetValue(false));

            Target.AddItem(new MenuItem("Chế độ chọn mục tiêu", "Mục tiêu").SetValue(new StringList(new string[] { "Optional", "Selected", "Priority", "Normal" })));
            Target.AddItem(new MenuItem("Orbwalk phạm vi bị động", "Orbwalk phạm vi bị động").SetValue(new Slider(300, 250, 500)));
            Target.AddItem(new MenuItem("Tập trung mục tiêu", "Tập trung mục tiêu").SetValue(false));
            Target.AddItem(new MenuItem("Note1", "Tùy chỉnh những gì bạn muốn trong menu"));
            Target.AddItem(new MenuItem("Note2", "Hãy nhớ rằng Orbwalk chỉ hoạt động bị động tại chỗ"));
            Target.AddItem(new MenuItem("Note3", "trong \" Combo Orbwalk để bị động\" chế độ có thể tìm thấy"));
            Target.AddItem(new MenuItem("Note4", "trong orbwalker menu!"));

            PriorityMode.AddItem(new MenuItem("Phạm vi mục tiêu ưu tiên", "Phạm vi ưu tiên").SetValue(new Slider(1000, 300, 1000)));
            PriorityMode.AddItem(new MenuItem("Ưu tiên orbwalk để bị động", "Orbwalk để bị động").SetValue(true));
            PriorityMode.AddItem(new MenuItem("Ưu tiên Mục tiêu dưới trụ", "Dứoi trụ").SetValue(true));
            foreach (var hero in HeroManager.Enemies)
            {
                PriorityMode.AddItem(new MenuItem("Priority" + hero.ChampionName, hero.ChampionName).SetValue(new Slider(2, 1, 5)));
            }

            OptionalMode.AddItem(new MenuItem("Phạm vi mục tieu tùy chọn", "Phạm vi tùy chọn").SetValue(new Slider(1000, 300, 1000)));
            OptionalMode.AddItem(new MenuItem("Tùy chọn mục tiêu bị động", "Orbwalk để bị động").SetValue(true));
            OptionalMode.AddItem(new MenuItem("Tùy chọn mục tiêu dưới trụ", "Under Tower").SetValue(false));
            OptionalMode.AddItem(new MenuItem("Phím thay đổi mục tiêu tùy chọn", "Switch Target Key").SetValue(new KeyBind('T', KeyBindType.Press)));
            OptionalMode.AddItem(new MenuItem("Note5", "Hoặc nhấp chuột trái để chọn mục tiêu muốn đổi"));

            SelectedMode.AddItem(new MenuItem("Phạm vi Mục tiêu đã chọn", "Phạm vi chọn").SetValue(new Slider(1000, 300, 1000)));
            SelectedMode.AddItem(new MenuItem("Mục tiêu chọn Orbwalk để bị động", "Orbwalk để bị động").SetValue(true));
            SelectedMode.AddItem(new MenuItem("Mục tiêu chọn dưới trụ", "Under Tower").SetValue(false));
            SelectedMode.AddItem(new MenuItem("Mục tiêu sẽ chuyển Tùy chọn mục tiêu nếu không được chọn", "Switch to Optional if no target").SetValue(true));

            LaneClear.AddItem(new MenuItem("Dùng E Đẩy đường", "Bật E").SetValue(true));
            LaneClear.AddItem(new MenuItem("Dùng Rìu mãng xà", "Bật Rìu mãng xà").SetValue(true));
            LaneClear.AddItem(new MenuItem("Mana thấp nhất để đẩy", "Mana thấp nhất").SetValue(new Slider(40, 0, 100)));

            JungClear.AddItem(new MenuItem("Dùng E Fam rừng", "Bật E").SetValue(true));
            JungClear.AddItem(new MenuItem("Dùng Rìu mãng xà", "Bật Rìu mãng xà").SetValue(true));
            JungClear.AddItem(new MenuItem("Mana thấp nhất để Fam", "Mana thấp nhất").SetValue(new Slider(40, 0, 100)));

            Misc.AddItem(new MenuItem("Vượt tường","Vượt tường").SetValue(new KeyBind('H',KeyBindType.Press)));

            Draw.AddItem(new MenuItem("Vòng Q", "Vòng Q").SetValue(false));
            Draw.AddItem(new MenuItem("Vòng W", "Draw W").SetValue(false));
            Draw.AddItem(new MenuItem("Vòng Mục tiêu tùy chọn", "Vòng Mục tiêu tùy chọn").SetValue(true));
            Draw.AddItem(new MenuItem("Vòng Mục tiêu được chọn", "Vòng Mục tiêu được chọn").SetValue(true));
            Draw.AddItem(new MenuItem("Vòng Mục tiêu ưu tiên", "Vòng Mục tiêu ưu tiên").SetValue(true));
            Draw.AddItem(new MenuItem("Vòng Mục tiêu", "Vòng Mục tiêu").SetValue(true));
            Draw.AddItem(new MenuItem("Vòng Quan trọng", "Vòng Quan trọng").SetValue(false));
            Draw.AddItem(new MenuItem("Vòng Damage Nhanh", "Vòng Damage nhanh").SetValue(false)).ValueChanged += DrawHP_ValueChanged;

            if (HeroManager.Enemies.Any())
            {
                Evade.Evade.Init();
                EvadeTarget.Init();
                TargetedNoMissile.Init();
                OtherSkill.Init();
            }
            OrbwalkLastClick.Init();
            Menu.AddToMainMenu();
            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += Drawing_OnEndScene;

            //GameObject.OnCreate += GameObject_OnCreate;
            Game.OnUpdate += Game_OnGameUpdate;
            Orbwalking.AfterAttack += AfterAttack;
            Orbwalking.AfterAttackNoTarget += Orbwalking_AfterAttackNoTarget;
            Orbwalking.OnAttack += OnAttack;
            Obj_AI_Base.OnProcessSpellCast += oncast;
            Game.OnWndProc += Game_OnWndProc;
            //Utility.HpBarDamageIndicator.DamageToUnit = GetFastDamage;
            //Utility.HpBarDamageIndicator.Enabled = DrawHP;
            CustomDamageIndicator.Initialize(GetFastDamage);
            CustomDamageIndicator.Enabled = DrawHP;

            //evade
            FioraProject.Evade.Evade.Evading += EvadeSkillShots.Evading;
            Game.PrintChat("Welcome to FioraWorld");
        }

        // events 
        public static void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (!unit.IsMe)
                return;
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.OrbwalkPassive
                || OrbwalkLastClickActive)
            {
                if (Ecombo && E.IsReady())
                {
                    E.Cast();
                }
                else if (HasItem())
                {
                    CastItem();
                }
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed && (unit is Obj_AI_Hero))
            {
                if (Eharass && E.IsReady() && Player.ManaPercent >= Manaharass)
                {
                    E.Cast();
                }
                else if (HasItem())
                {
                    CastItem();
                }
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                // jungclear
                if (EJclear && E.IsReady() && Player.Mana * 100 / Player.MaxMana >= ManaJclear && !Orbwalker.ShouldWait()
                    && Player.Position.CountMinionsInRange(Orbwalking.GetRealAutoAttackRange(Player) + 200, true) >= 1)
                {
                    E.Cast();
                }
                else if (TimatJClear && HasItem() && !Orbwalker.ShouldWait()
                    && Player.Position.CountMinionsInRange(Orbwalking.GetRealAutoAttackRange(Player) + 200, true) >= 1)
                {
                    CastItem();
                }
                // laneclear
                if (ELclear && E.IsReady() && Player.Mana * 100 / Player.MaxMana >= ManaLclear && !Orbwalker.ShouldWait()
                    && Player.Position.CountMinionsInRange(Orbwalking.GetRealAutoAttackRange(Player) + 200, false) >= 1)
                {
                    E.Cast();
                }
                else if (TimatLClear && HasItem() && !Orbwalker.ShouldWait()
                    && Player.Position.CountMinionsInRange(Orbwalking.GetRealAutoAttackRange(Player) + 200, false) >= 1)
                {
                    CastItem();
                }
            }

        }
        private static void Orbwalking_AfterAttackNoTarget(AttackableUnit unit, AttackableUnit target)
        {
            if (!unit.IsMe)
                return;
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.OrbwalkPassive
                || OrbwalkLastClickActive)
            {
                if (Ecombo && E.IsReady() && Player.CountEnemiesInRange(Orbwalking.GetRealAutoAttackRange(Player) + 200) >= 1)
                {
                    E.Cast();
                }
                else if (HasItem() && Player.CountEnemiesInRange(Orbwalking.GetRealAutoAttackRange(Player) + 200) >= 1)
                {
                    CastItem();
                }
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed && (unit is Obj_AI_Hero))
            {
                if (Eharass && E.IsReady() && Player.ManaPercent >= Manaharass
                    && Player.CountEnemiesInRange(Orbwalking.GetRealAutoAttackRange(Player) + 200) >= 1)
                {
                    E.Cast();
                }
                else if (HasItem() && Player.CountEnemiesInRange(Orbwalking.GetRealAutoAttackRange(Player) + 200) >= 1)
                {
                    CastItem();
                }
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                // jungclear
                if (EJclear && E.IsReady() && Player.Mana * 100 / Player.MaxMana >= ManaJclear && !Orbwalker.ShouldWait()
                    && Player.Position.CountMinionsInRange(Orbwalking.GetRealAutoAttackRange(Player) + 200, true) >= 1)
                {
                    E.Cast();
                }
                else if (TimatJClear && HasItem() && !Orbwalker.ShouldWait()
                    && Player.Position.CountMinionsInRange(Orbwalking.GetRealAutoAttackRange(Player) + 200, true) >= 1)
                {
                    CastItem();
                }
                // laneclear
                if (ELclear && E.IsReady() && Player.Mana * 100 / Player.MaxMana >= ManaLclear && !Orbwalker.ShouldWait()
                    && Player.Position.CountMinionsInRange(Orbwalking.GetRealAutoAttackRange(Player) + 200, false) >= 1)
                {
                    E.Cast();
                }
                else if (TimatLClear && HasItem() && !Orbwalker.ShouldWait()
                    && Player.Position.CountMinionsInRange(Orbwalking.GetRealAutoAttackRange(Player) + 200, false) >= 1)
                {
                    CastItem();
                }
            }
        }

        public static void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead)
                return;
            FioraPassiveUpdate();
            OrbwalkToPassive();
            WallJump();
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.OrbwalkPassive)
            {
                Combo();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                Harass();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {

            }
        }
        public static void oncast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var spell = args.SData;
            if (!sender.IsMe)
                return;
            if (spell.Name.Contains("ItemTiamatCleave"))
            {

            }
            if (spell.Name.Contains("FioraQ"))
            {

            }
            if (spell.Name == "FioraE")
            {

                Orbwalking.ResetAutoAttackTimer();
            }
            if (spell.Name == "ItemTitanicHydraCleave")
            {
                Orbwalking.ResetAutoAttackTimer();
            }
            if (spell.Name.ToLower().Contains("fiorabasicattack"))
            {
            }

        }
        public static void OnAttack(AttackableUnit unit, AttackableUnit target)
        {

            if (unit.IsMe
                && (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo
                || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.OrbwalkPassive
                || OrbwalkLastClickActive))
            {
                if (ItemData.Youmuus_Ghostblade.GetItem().IsReady())
                    ItemData.Youmuus_Ghostblade.GetItem().Cast();
            }

        }


        //harass
        public static bool Qharass { get { return Menu.Item("Use Q Harass").GetValue<bool>(); } }
        private static bool Eharass { get { return Menu.Item("Use E Harass").GetValue<bool>(); } }
        public static bool CastQGapCloseHarass { get { return Menu.Item("Use Q Harass Gap").GetValue<bool>(); } }
        public static bool CastQPrePassiveHarass { get { return Menu.Item("Use Q Harass Pre Pass").GetValue<bool>(); } }
        public static bool CastQPassiveHarasss { get { return Menu.Item("Use Q Harass Pass").GetValue<bool>(); } }
        public static int Manaharass { get { return Menu.Item("Mana Harass").GetValue<Slider>().Value; } }

        //combo
        public static bool Qcombo { get { return Menu.Item("Use Q Combo").GetValue<bool>(); } }
        private static bool Ecombo { get { return Menu.Item("Use E Combo").GetValue<bool>(); } }
        public static bool CastQGapCloseCombo { get { return Menu.Item("Use Q Combo Gap").GetValue<bool>(); } }
        public static bool CastQPrePassiveCombo { get { return Menu.Item("Use Q Combo Pre Pass").GetValue<bool>(); } }
        public static bool CastQPassiveCombo { get { return Menu.Item("Use Q Combo Pass").GetValue<bool>(); } }
        public static bool CastQMinionGapCloseCombo { get { return Menu.Item("Use Q Combo Gap Minion").GetValue<bool>(); } }
        public static int ValueQMinionGapCloseCombo { get { return Menu.Item("Use Q Combo Gap Minion Value").GetValue<Slider>().Value; } }
        public static bool Rcombo { get { return Menu.Item("Use R Combo").GetValue<bool>(); } }
        public static bool UseRComboLowHP { get { return Menu.Item("Use R Combo LowHP").GetValue<bool>(); } }
        public static int ValueRComboLowHP { get { return Menu.Item("Use R Combo LowHP Value").GetValue<Slider>().Value; } }
        public static bool UseRComboKillable { get { return Menu.Item("Use R Combo Killable").GetValue<bool>(); } }
        public static bool UseRComboOnTap { get { return Menu.Item("Use R Combo On Tap").GetValue<bool>(); } }
        public static bool RTapKeyActive { get { return Menu.Item("Use R Combo On Tap Key").GetValue<KeyBind>().Active; } }
        public static bool UseRComboAlways { get { return Menu.Item("Use R Combo Always").GetValue<bool>(); } }

        //jclear && lclear
        private static bool ELclear { get { return Menu.Item("Use E LClear").GetValue<bool>(); } }
        private static bool TimatLClear { get { return Menu.Item("Use Timat LClear").GetValue<bool>(); } }
        private static bool EJclear { get { return Menu.Item("Use E JClear").GetValue<bool>(); } }
        private static bool TimatJClear { get { return Menu.Item("Use Timat JClear").GetValue<bool>(); } }
        public static int ManaJclear { get { return Menu.Item("minimum Mana JC").GetValue<Slider>().Value; } }
        public static int ManaLclear { get { return Menu.Item("minimum Mana LC").GetValue<Slider>().Value; } }

        //orbwalkpassive
        private static float OrbwalkToPassiveRange { get { return Menu.Item("Orbwalk To Passive Range").GetValue<Slider>().Value; } }
        private static bool OrbwalkToPassiveTargeted { get { return Menu.Item("Selected Orbwalk to Passive").GetValue<bool>(); } }
        private static bool OrbwalkToPassiveOptional { get { return Menu.Item("Optional Orbwalk to Passive").GetValue<bool>(); } }
        private static bool OrbwalkToPassivePriority { get { return Menu.Item("Priority Orbwalk to Passive").GetValue<bool>(); } }
        private static bool OrbwalkTargetedUnderTower { get { return Menu.Item("Selected Under Tower").GetValue<bool>(); } }
        private static bool OrbwalkOptionalUnderTower { get { return Menu.Item("Optional Under Tower").GetValue<bool>(); } }
        private static bool OrbwalkPriorityUnderTower { get { return Menu.Item("Priority Under Tower").GetValue<bool>(); } }

        // orbwalklastclick
        public static bool OrbwalkLastClickActive { get { return Menu.Item("Orbwalk Last Right Click").GetValue<KeyBind>().Active; } }

        #region Drawing
        private static bool DrawQ { get { return Menu.Item("Draw Q").GetValue<bool>(); } }
        private static bool DrawW { get { return Menu.Item("Draw W").GetValue<bool>(); } }
        private static bool DrawQcast { get { return Menu.Item("Draw Q cast").GetValue<bool>(); } }
        private static bool DrawOptionalRange { get { return Menu.Item("Draw Optional Range").GetValue<bool>(); } }
        private static bool DrawSelectedRange { get { return Menu.Item("Draw Selected Range").GetValue<bool>(); } }
        private static bool DrawPriorityRange { get { return Menu.Item("Draw Priority Range").GetValue<bool>(); } }
        private static bool DrawTarget { get { return Menu.Item("Draw Target").GetValue<bool>(); } }
        private static bool DrawHP { get { return Menu.Item("Draw Fast Damage").GetValue<bool>(); } }
        private static bool DrawVitals { get { return Menu.Item("Draw Vitals").GetValue<bool>(); } }
        private static void DrawHP_ValueChanged(object sender, OnValueChangeEventArgs e)
        {
            if (sender != null)
            {
                //Utility.HpBarDamageIndicator.Enabled = e.GetNewValue<bool>();
                CustomDamageIndicator.Enabled = e.GetNewValue<bool>();
            }
        }
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead)
                return;
            if (DrawQ)
                Render.Circle.DrawCircle(Player.Position, 400, Color.Green);
            if (DrawW)
            {
                Render.Circle.DrawCircle(Player.Position, W.Range, Color.Green);
            }
            if (DrawOptionalRange && TargetingMode == TargetMode.Optional)
            {
                Render.Circle.DrawCircle(Player.Position, OptionalRange, Color.DeepPink);
            }
            if (DrawSelectedRange && TargetingMode == TargetMode.Selected)
            {
                Render.Circle.DrawCircle(Player.Position, SelectedRange, Color.DeepPink);
            }
            if (DrawPriorityRange && TargetingMode == TargetMode.Priority)
            {
                Render.Circle.DrawCircle(Player.Position, PriorityRange, Color.DeepPink);
            }
            if (DrawTarget && TargetingMode != TargetMode.Normal)
            {
                var hero = GetTarget();
                if (hero != null)
                    Render.Circle.DrawCircle(hero.Position, 75, Color.Yellow, 5);
            }
            if (DrawVitals && TargetingMode != TargetMode.Normal)
            {
                var hero = GetTarget();
                if (hero != null)
                {
                    var status = hero.GetPassiveStatus(0f);
                    if (status.HasPassive && status.PassivePredictedPositions.Any())
                    {
                        foreach (var x in status.PassivePredictedPositions)
                        {
                            Render.Circle.DrawCircle(x.To3D(), 50, Color.Yellow);
                        }
                    }
                }
            }
            if (activewalljump)
            {
                var Fstwall = GetFirstWallPoint(Player.Position.To2D(), Game.CursorPos.To2D());
                if (Fstwall != null)
                {
                    var firstwall =((Vector2)Fstwall);
                    var pos = firstwall.Extend(Game.CursorPos.To2D(), 100);
                    var Lstwall = GetLastWallPoint(firstwall, Game.CursorPos.To2D());
                    if (Lstwall != null)
                    {
                        var lastwall = ((Vector2)Lstwall);
                        if (InMiddileWall(firstwall,lastwall))
                        {
                        for (int i = 0; i <= 359; i++)
                        {
                            var pos1 = pos.RotateAround(firstwall, i);
                            var pos2 = firstwall.Extend(pos1, 400);
                            if (pos1.InTheCone(firstwall, Game.CursorPos.To2D(), 60) && pos1.IsWall() && !pos2.IsWall())
                            {
                                Render.Circle.DrawCircle(firstwall.To3D(), 50, Color.Green);
                                goto Finish;
                            }
                        }

                        Render.Circle.DrawCircle(firstwall.To3D(), 50, Color.Red);
                        }
                    }
                }
            Finish: ;
            }

        }
        private static void Drawing_OnEndScene(EventArgs args)
        {
        }

        #endregion Drawing

        #region WallJump
        public static bool usewalljump = true;
        public static bool activewalljump { get { return Menu.Item("WallJump").GetValue<KeyBind>().Active; } }
        public static int movetick;
        public static void WallJump()
        {
            if (usewalljump && activewalljump)
            {
                var Fstwall = GetFirstWallPoint(Player.Position.To2D(), Game.CursorPos.To2D());
                if (Fstwall != null)
                {
                    var firstwall = ((Vector2)Fstwall);
                    var Lstwall = GetLastWallPoint(firstwall, Game.CursorPos.To2D());
                    if (Lstwall != null)
                    {
                        var lastwall = ((Vector2)Lstwall);
                        if (InMiddileWall(firstwall, lastwall))
                        {
                            var y = Player.Position.Extend(Game.CursorPos, 30);
                            for (int i = 20; i <= 300; i = i + 20)
                            {
                                if (Utils.GameTimeTickCount - movetick < (70 + Math.Min(60, Game.Ping)))
                                    break;
                                if (Player.Distance(Game.CursorPos) <= 1200 && Player.Position.To2D().Extend(Game.CursorPos.To2D(), i).IsWall())
                                {
                                    Player.IssueOrder(GameObjectOrder.MoveTo, Player.Position.To2D().Extend(Game.CursorPos.To2D(), i - 20).To3D());
                                    movetick = Utils.GameTimeTickCount;
                                    break;
                                }
                                Player.IssueOrder(GameObjectOrder.MoveTo,
                                    Player.Distance(Game.CursorPos) <= 1200 ?
                                    Player.Position.To2D().Extend(Game.CursorPos.To2D(), 200).To3D() :
                                    Game.CursorPos);
                            }
                            if (y.IsWall() && Prediction.GetPrediction(Player, 500).UnitPosition.Distance(Player.Position) <= 10 && Q.IsReady())
                            {
                                var pos = Player.Position.To2D().Extend(Game.CursorPos.To2D(), 100);
                                for (int i = 0; i <= 359; i++)
                                {
                                    var pos1 = pos.RotateAround(Player.Position.To2D(), i);
                                    var pos2 = Player.Position.To2D().Extend(pos1, 400);
                                    if (pos1.InTheCone(Player.Position.To2D(), Game.CursorPos.To2D(), 60) && pos1.IsWall() && !pos2.IsWall())
                                    {
                                        Q.Cast(pos2);
                                    }

                                }
                            }
                        }
                        else if (Utils.GameTimeTickCount - movetick >= (70 + Math.Min(60, Game.Ping)))
                        {
                            Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                            movetick = Utils.GameTimeTickCount;
                        }
                    }
                    else if (Utils.GameTimeTickCount - movetick >= (70 + Math.Min(60, Game.Ping)))
                    {
                        Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                        movetick = Utils.GameTimeTickCount;
                    }
                }
                else if (Utils.GameTimeTickCount - movetick >= (70 + Math.Min(60, Game.Ping)))
                {
                    Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                    movetick = Utils.GameTimeTickCount;
                }
            }
        }
        public static Vector2? GetFirstWallPoint(Vector2 from, Vector2 to, float step = 25)
        {
            var direction = (to - from).Normalized();

            for (float d = 0; d < from.Distance(to); d = d + step)
            {
                var testPoint = from + d * direction;
                var flags = NavMesh.GetCollisionFlags(testPoint.X, testPoint.Y);
                if (flags.HasFlag(CollisionFlags.Wall) || flags.HasFlag(CollisionFlags.Building))
                {
                    return from + (d - step) * direction;
                }
            }

            return null;
        }
        public static Vector2? GetLastWallPoint (Vector2 from, Vector2 to , float step = 25)
        {
            var direction = (to - from).Normalized();
            var Fstwall = GetFirstWallPoint(from, to);
            if (Fstwall != null)
            {
                var firstwall = ((Vector2)Fstwall);
                for (float d = step; d < firstwall.Distance(to) + 1000; d = d + step)
                {
                    var testPoint = firstwall + d * direction;
                    var flags = NavMesh.GetCollisionFlags(testPoint.X, testPoint.Y);
                    if (!flags.HasFlag(CollisionFlags.Wall) && !flags.HasFlag(CollisionFlags.Building))
                    //if (!testPoint.IsWall())
                    {
                        return firstwall + d * direction;
                    }
                }
            }

            return null;
        }
        public static bool InMiddileWall (Vector2 firstwall, Vector2 lastwall)
        {
            var midwall = new Vector2((firstwall.X + lastwall.X)/2,(firstwall.Y + lastwall.Y)/2);
            var point = midwall.Extend(Game.CursorPos.To2D(), 50);
            for (int i = 0; i <= 350; i = i + 10  )
            {
                var testpoint = point.RotateAround(midwall, i);
                var flags = NavMesh.GetCollisionFlags(testpoint.X,testpoint.Y);
                if (!flags.HasFlag(CollisionFlags.Wall) && !flags.HasFlag(CollisionFlags.Building))
                {
                    return false;
                }
            }
            return true;
        }
        #endregion WallJump

        #region OrbwalkToPassive
        private static void OrbwalkToPassive()
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.OrbwalkPassive)
            {
                var target = GetTarget(OrbwalkToPassiveRange);
                if (target.IsValidTarget(OrbwalkToPassiveRange) && !target.IsZombie)
                {
                    var status = target.GetPassiveStatus(0);
                    if (Player.Position.To2D().Distance(target.Position.To2D()) <= OrbwalkToPassiveRange && status.HasPassive
                        && ((TargetingMode == TargetMode.Selected && OrbwalkToPassiveTargeted && (OrbwalkTargetedUnderTower || !Player.UnderTurret(true)))
                        || (TargetingMode == TargetMode.Optional && OrbwalkToPassiveOptional && (OrbwalkOptionalUnderTower || !Player.UnderTurret(true)))
                        || (TargetingMode == TargetMode.Priority && OrbwalkToPassivePriority && (OrbwalkPriorityUnderTower || !Player.UnderTurret(true)))))
                    {
                        var point = status.PassivePredictedPositions.OrderBy(x => x.Distance(Player.Position.To2D())).FirstOrDefault();
                        point = point.IsValid() ? point : Game.CursorPos.To2D();
                        Orbwalker.SetOrbwalkingPoint(point.To3D());
                    }
                    else Orbwalker.SetOrbwalkingPoint(Game.CursorPos);
                }
                else Orbwalker.SetOrbwalkingPoint(Game.CursorPos);
            }
            else Orbwalker.SetOrbwalkingPoint(Game.CursorPos);
        }
        #endregion OrbwalkToPassive
    }
    // jax E, riven W,dianaE,maokaiR, ---------------> solving
    // :D Vladimir R Zed R tris E, morganaR,nocturn E,Karthus R, FizzR,karmaW leblancE------> solving
    //jax W, yorick Q, poppy Q rengar Q   nautilus, ---------> solving
    //,  riven Q3,  leesin Q2,   nocturne R, ramus Q, -----> solving
    // kalista E,kennen W, -----> solving

    // LeblancSoulShackle , LeblancShoulShackleM nautiluspassivecheck kennenmarkofstorm  kalistaexpungemarker--- done
    // nautilusgrandlinetarget NocturneUnspeakableHorror GrandLineSeeker (nau R) nocturneparanoiadash---- done
    // , ,  ,  , RivenFengShuiEngine , ====> done
}
