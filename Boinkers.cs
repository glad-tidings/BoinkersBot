using System.Text.Json.Serialization;

namespace Boinkers
{
    public class BoinkersQuery
    {
        public int Index { get; set; }
        public string Name { get; set; } = string.Empty;
        public string API_ID { get; set; } = string.Empty;
        public string API_HASH { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Auth { get; set; } = string.Empty;
        public bool Active { get; set; }
        public bool Booster { get; set; }
        public bool Upgrade { get; set; }
        public bool Spin { get; set; }
        public int[] SpinCount { get; set; } = [];
        public int[] SpinSleep { get; set; } = [];
        public bool Task { get; set; }
        public int[] TaskSleep { get; set; } = [];
        public bool Raffle { get; set; }
        public int[] DaySleep { get; set; } = [];
        public int[] NightSleep { get; set; } = [];
    }

    public class BoinkersConfigResponse
    {
        [JsonPropertyName("adsgramBlockId")]
        public string AdsgramBlockId { get; set; } = string.Empty;
        [JsonPropertyName("isAnimateClouds")]
        public bool IsAnimateClouds { get; set; }
        [JsonPropertyName("isShowStars")]
        public bool IsShowStars { get; set; }
        [JsonPropertyName("versionHash")]
        public string VersionHash { get; set; } = string.Empty;
        [JsonPropertyName("liveOps")]
        public List<BoinkersConfigLiveOps> LiveOps { get; set; } = [];
    }

    public class BoinkersConfigLiveOps
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; } = string.Empty;
        [JsonPropertyName("liveOpName")]
        public string LiveOpName { get; set; } = string.Empty;
        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }
    }

    public class BoinkersLoginRequest
    {
        [JsonPropertyName("initDataString")]
        public string InitDataString { get; set; } = string.Empty;
        [JsonPropertyName("sessionParams")]
        public BoinkersLoginSessionParams SessionParams { get; set; } = new();
    }

    public class BoinkersLoginSessionParams
    {

    }

    public class BoinkersLoginResponse
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; } = string.Empty;
        [JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty;
    }

    public class BoinkersUserInfoResponse
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; } = string.Empty;
        [JsonPropertyName("userName")]
        public string UserName { get; set; } = string.Empty;
        [JsonPropertyName("currencySoft")]
        public int CurrencySoft { get; set; }
        [JsonPropertyName("currencyCrypto")]
        public double? CurrencyCrypto { get; set; }
        [JsonPropertyName("boinkers")]
        public BoinkersUserInfoBoinkers Boinkers { get; set; } = new();
        [JsonPropertyName("gamesEnergy")]
        public BoinkersUserInfoGamesEnergy GamesEnergy { get; set; } = new();
        [JsonPropertyName("raffle")]
        public BoinkersUserInfoRaffle Raffle { get; set; } = new();
        [JsonPropertyName("rank")]
        public int Rank { get; set; }
    }

    public class BoinkersUserInfoBoinkers
    {
        [JsonPropertyName("currentBoinkerProgression")]
        public BoinkersUserInfoBoinkersCurrentBoinkerProgression CurrentBoinkerProgression { get; set; } = new();
        [JsonPropertyName("booster")]
        public BoinkersUserInfoBoinkersBooster Booster { get; set; } = new();
        [JsonPropertyName("completedBoinkers")]
        public int CompletedBoinkers { get; set; }
    }

    public class BoinkersUserInfoBoinkersCurrentBoinkerProgression
    {
        [JsonPropertyName("level")]
        public int Level { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
    }

    public class BoinkersUserInfoBoinkersBooster
    {
        [JsonPropertyName("x2")]
        public BoinkersUserInfoBoinkersBoosterX2 X2 { get; set; } = new();
    }

    public class BoinkersUserInfoBoinkersBoosterX2
    {
        [JsonPropertyName("lastTimeFreeOptionClaimed")]
        public DateTime? LastTimeFreeOptionClaimed { get; set; }
    }

    public class BoinkersUserInfoGamesEnergy
    {
        [JsonPropertyName("moonMine")]
        public BoinkersUserInfoGamesEnergyItem MoonMine { get; set; } = new();
        [JsonPropertyName("slotMachine")]
        public BoinkersUserInfoGamesEnergyItem SlotMachine { get; set; } = new();
        [JsonPropertyName("wheelOfFortune")]
        public BoinkersUserInfoGamesEnergyItem WheelOfFortune { get; set; } = new();
        [JsonPropertyName("elevators")]
        public BoinkersUserInfoGamesEnergyItem Elevators { get; set; } = new();
    }

    public class BoinkersUserInfoRaffle
    {
        [JsonPropertyName("milestoneReached")]
        public int MilestoneReached { get; set; }
        [JsonPropertyName("tickets")]
        public int Tickets { get; set; }
    }

    public class BoinkersUserInfoGamesEnergyItem
    {
        [JsonPropertyName("energy")]
        public int Energy { get; set; }
        [JsonPropertyName("freeEnergyUsed")]
        public int FreeEnergyUsed { get; set; }
        [JsonPropertyName("betsDone")]
        public int BetsDone { get; set; }
        [JsonPropertyName("energyUsed")]
        public int EnergyUsed { get; set; }
    }

    public class BoinkersBoosterRequest
    {
        [JsonPropertyName("multiplier")]
        public int Multiplier { get; set; }
        [JsonPropertyName("optionNumber")]
        public int OptionNumber { get; set; }
    }

    public class BoinkersWheelOfFortuneResponse
    {
        [JsonPropertyName("expire")]
        public DateTime Expire { get; set; }
        [JsonPropertyName("liveOpId")]
        public string LiveOpId { get; set; } = string.Empty;
    }

    public class BoinkersWheelOfFortuneRequest
    {
        [JsonPropertyName("liveOpId")]
        public string LiveOpId { get; set; } = string.Empty;
    }

    public class BoinkersSpinResponse
    {
        [JsonPropertyName("prize")]
        public BoinkersSpinPrize Prize { get; set; } = new();
        [JsonPropertyName("outcome")]
        public string Outcome { get; set; } = string.Empty;
    }

    public class BoinkersSpinPrize
    {
        [JsonPropertyName("prizeType")]
        public int PrizeType { get; set; }
        [JsonPropertyName("prizeName")]
        public string PrizeName { get; set; } = string.Empty;
        [JsonPropertyName("prizeParticles")]
        public int PrizeParticles { get; set; }
        [JsonPropertyName("prizeValue")]
        public int PrizeValue { get; set; }
        [JsonPropertyName("prizeTypeName")]
        public string PrizeTypeName { get; set; } = string.Empty;
    }

    public class BoinkersRaffleResponse
    {
        [JsonPropertyName("userRaffleData")]
        public BoinkersRaffleUserRaffleData UserRaffleData { get; set; } = new();
    }

    public class BoinkersRaffleUserRaffleData
    {
        [JsonPropertyName("milestoneReached")]
        public int MilestoneReached { get; set; }
        [JsonPropertyName("tickets")]
        public int Tickets { get; set; }
        [JsonPropertyName("raffleId")]
        public string RaffleId { get; set; } = string.Empty;
    }

    public class ProxyType
    {
        [JsonPropertyName("Index")]
        public int Index { get; set; }
        [JsonPropertyName("Proxy")]
        public string Proxy { get; set; } = string.Empty;
    }

    public class Httpbin
    {
        [JsonPropertyName("origin")]
        public string Origin { get; set; } = string.Empty;
    }
}
