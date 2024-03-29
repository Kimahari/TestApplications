﻿using SimpleDbContextPooling.Data.Internal;
using SimpleDbContextPooling.Data.Models;

namespace SimpleDbContextPooling.Data;

class TestDbContext : PoolAbleDbContext {

    public TestDbContext(DbContextOptions options) : base(options) {

    }

    protected TestDbContext() {

    }

    public DbSet<MyClass> Classes { get; set; }
}
