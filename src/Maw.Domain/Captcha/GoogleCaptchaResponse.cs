using System;


// don't follow conventions for google responses
#pragma warning disable IDE1006
#pragma warning disable CA1707

public class GoogleCaptchaResponse {
    public bool success { get; set;}
    public DateTime challenge_ts { get; set; }
    public string hostname { get; set; }
}
