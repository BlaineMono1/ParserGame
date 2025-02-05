using System;
using Business.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DataBaseToAccess
{
    public class BaseDbContext:DbContext
    {
        public BaseDbContext(DbContextOptions<BaseDbContext> options) : base(options) { }

        public DbSet<AddOn> AddOns { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Edition> Editions { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<FavoriteItem> FavoriteItems { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<GroupAddOn> GroupAddOns { get; set; }
        public DbSet<LoyaltyCashback> LoyaltyCashbacks { get; set; }
        public DbSet<LoyaltyCurrency> LoyaltyCurrencies { get; set; }
        public DbSet<LoyaltyOrder> LoyaltyOrders { get; set; }
        public DbSet<LoyaltyProduct> LoyaltyProducts { get; set; }
        public DbSet<LoyaltySetting> LoyaltySettings { get; set; }
        public DbSet<LoyaltyTransactionHistory> LoyaltyTransactionHistories { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderProductItem> OrdersProductItems { get; set; }
        public DbSet<PriceSettingSubscription> PriceSettings { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductTransactionHistory> ProductTransactionHistories { get; set; }
        public DbSet<ProductTransactionItem> ProductTransactionItems { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<SettingPrice> SettingsPrice { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<User> Users { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);
        }
    }
}
