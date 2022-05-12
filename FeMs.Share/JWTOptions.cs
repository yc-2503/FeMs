﻿namespace FeMs.Share
{
    public class JWTOptions
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpireSeconds { get; set; }
    }
}