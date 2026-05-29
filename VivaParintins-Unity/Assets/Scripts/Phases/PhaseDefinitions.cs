// Definições das 8 fases da rota do torcedor até o Bumbódromo.
// Usado pelo PhaseRunner para carregar configuração de cada fase.

using UnityEngine;

namespace VivaParintins.Phases
{
    public enum PhaseId
    {
        PortoParintins   = 0,
        FeiraArtesanatos = 1,
        MercadoMunicipal = 2,
        OrlaParintins    = 3,
        ComunasBar       = 4,
        CurralDoBoi      = 5,
        PracaDoisBois    = 6,
        Bumbodromo       = 7
    }

    [System.Serializable]
    public class PhaseConfig
    {
        public PhaseId    id;
        public string     displayName;
        [TextArea] public string description;
        public string     sceneName;          // Nome da cena Unity (Build Settings)
        public float      runnerSpeed;        // Velocidade base do runner nessa fase
        public int        laneCount;
        public bool       hasWindPhysics;     // Fase 4: Orla com vento
        public bool       hasBeatSync;        // Fase 5: Comunas — música sincronizada
        public bool       hasToadaBuff;       // Fase 6: Curral — buff de toada
        public bool       isBossPhase;        // Fase 8: Bumbódromo final
        public int        coinGoal;           // Meta de miçangas para 3 estrelas
        public int        timeLimitSeconds;
    }

    // ScriptableObject que carrega a lista de fases no Inspector
    [CreateAssetMenu(menuName = "VivaParintins/Phase List")]
    public class PhaseList : ScriptableObject
    {
        public PhaseConfig[] phases = new PhaseConfig[]
        {
            new PhaseConfig {
                id              = PhaseId.PortoParintins,
                displayName     = "Porto de Parintins",
                description     = "Desembarque dos barcos! Desvie de redes e malas na balsa.",
                sceneName       = "Phase_Porto",
                runnerSpeed     = 6f,
                laneCount       = 3,
                coinGoal        = 50,
                timeLimitSeconds= 60
            },
            new PhaseConfig {
                id              = PhaseId.FeiraArtesanatos,
                displayName     = "Feira de Artesanatos",
                description     = "Labirinto de barracas! Colete penas e sementes.",
                sceneName       = "Phase_Feira",
                runnerSpeed     = 7f,
                laneCount       = 3,
                coinGoal        = 70,
                timeLimitSeconds= 55
            },
            new PhaseConfig {
                id              = PhaseId.MercadoMunicipal,
                displayName     = "Mercado Municipal",
                description     = "Colete Tacacá e Guaraná em pó para ganhar buffs!",
                sceneName       = "Phase_Mercado",
                runnerSpeed     = 7.5f,
                laneCount       = 3,
                coinGoal        = 80,
                timeLimitSeconds= 55
            },
            new PhaseConfig {
                id              = PhaseId.OrlaParintins,
                displayName     = "Orla de Parintins",
                description     = "Vento forte na beira do rio empurra o torcedor! Resista!",
                sceneName       = "Phase_Orla",
                runnerSpeed     = 8f,
                laneCount       = 3,
                hasWindPhysics  = true,
                coinGoal        = 90,
                timeLimitSeconds= 50
            },
            new PhaseConfig {
                id              = PhaseId.ComunasBar,
                displayName     = "Comunas Bar",
                description     = "Noite de festa! Reflexos rápidos — desvie de mesas e aglomerações.",
                sceneName       = "Phase_Comunas",
                runnerSpeed     = 9f,
                laneCount       = 3,
                hasBeatSync     = true,
                coinGoal        = 100,
                timeLimitSeconds= 50
            },
            new PhaseConfig {
                id              = PhaseId.CurralDoBoi,
                displayName     = "Curral do Boi",
                description     = "Encontre as figuras do seu boi e receba o Buff da Toada!",
                sceneName       = "Phase_Curral",
                runnerSpeed     = 9f,
                laneCount       = 3,
                hasToadaBuff    = true,
                coinGoal        = 110,
                timeLimitSeconds= 50
            },
            new PhaseConfig {
                id              = PhaseId.PracaDoisBois,
                displayName     = "Praça dos Dois Bois",
                description     = "Cruzamento de torcidas! Desvie das cores inimigas e converta neutros.",
                sceneName       = "Phase_Praca",
                runnerSpeed     = 10f,
                laneCount       = 5,
                coinGoal        = 130,
                timeLimitSeconds= 45
            },
            new PhaseConfig {
                id              = PhaseId.Bumbodromo,
                displayName     = "Bumbódromo — Grande Final",
                description     = "A arena está em chamas! Alta pontuação, timer agressivo. É agora ou nunca!",
                sceneName       = "Phase_Bumbodromo",
                runnerSpeed     = 12f,
                laneCount       = 5,
                isBossPhase     = true,
                coinGoal        = 200,
                timeLimitSeconds= 90
            }
        };
    }
}
