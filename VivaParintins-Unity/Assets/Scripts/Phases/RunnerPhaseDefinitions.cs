// Define as 6 fases do auto-runner estilo Super Mario Run.
// Cada fase termina num ponto turístico de Parintins com um personagem do boi.

using UnityEngine;

namespace VivaParintins.Phases
{
    public enum RunnerPhaseId
    {
        PortoParintins   = 0,
        MercadoMunicipal = 1,
        TribuIndigena    = 2,
        CurralDoBoi      = 3,  // muda nome conforme time (Cidade Garantido / Curral Caprichoso)
        PracaDoisBois    = 4,
        Bumbodromo       = 5
    }

    [System.Serializable]
    public class RunnerPhaseConfig
    {
        public RunnerPhaseId id;
        public string displayName;
        [TextArea]
        public string description;
        public string sceneName;

        [Header("Runner")]
        public float  startSpeed       = 7f;
        public float  acceleration     = 0.08f;   // velocidade aumenta ao longo da fase
        public float  levelLength      = 120f;    // metros até o fim da fase
        public int    totalCollectibles = 20;      // total de ❤️/⭐ na fase

        [Header("Dificuldade")]
        public float  gapFrequency     = 0.2f;    // chance de gap entre plataformas
        public float  obstacleFrequency = 0.3f;

        [Header("Landmark (fim de fase)")]
        public string landmarkNameGar;            // nome do ponto para torcedor Garantido
        public string landmarkNameCap;            // nome do ponto para torcedor Caprichoso
        public string characterGar;               // personagem Garantido que aparece
        public string characterCap;               // personagem Caprichoso que aparece
        public string characterRoleGar;
        public string characterRoleCap;
        [TextArea] public string quoteGar;
        [TextArea] public string quoteCap;
    }

    [CreateAssetMenu(menuName = "VivaParintins/Runner Phase List")]
    public class RunnerPhaseList : ScriptableObject
    {
        public RunnerPhaseConfig[] phases = new RunnerPhaseConfig[]
        {
            new RunnerPhaseConfig {
                id                  = RunnerPhaseId.PortoParintins,
                displayName         = "Porto de Parintins",
                description         = "A Ilha Mágica te espera! Corra pelo cais, pule sobre as caixas e embarque na festa!",
                sceneName           = "Runner_Porto",
                startSpeed          = 6f,
                acceleration        = 0.05f,
                levelLength         = 100f,
                totalCollectibles   = 15,
                gapFrequency        = 0.1f,
                obstacleFrequency   = 0.2f,
                landmarkNameGar     = "Cais do Garantido",
                landmarkNameCap     = "Cais do Caprichoso",
                characterGar        = "Israel Paulain",
                characterCap        = "Edmundo Oran",
                characterRoleGar    = "Apresentador",
                characterRoleCap    = "Apresentador",
                quoteGar            = "GARANTIDO! O boi do povo chegou na Ilha! ❤️",
                quoteCap            = "CAPRICHOSO! A estrela da Amazônia desembarcou! ⭐"
            },
            new RunnerPhaseConfig {
                id                  = RunnerPhaseId.MercadoMunicipal,
                description         = "Desvie das barracas, colete as frutas regionais e sinta o cheiro do tacacá!",
                sceneName           = "Runner_Mercado",
                startSpeed          = 7f,
                acceleration        = 0.06f,
                levelLength         = 110f,
                totalCollectibles   = 18,
                gapFrequency        = 0.15f,
                obstacleFrequency   = 0.25f,
                landmarkNameGar     = "Mercado do Coração",
                landmarkNameCap     = "Mercado da Estrela",
                characterGar        = "Raíra Lins",
                characterCap        = "Valentina Cid",
                characterRoleGar    = "Sinhazinha da Fazenda",
                characterRoleCap    = "Sinhazinha da Fazenda",
                quoteGar            = "Que cheiro bom! Isso é Parintins de verdade! ❤️",
                quoteCap            = "A fartura da Amazônia é nossa! Vai Caprichoso! ⭐"
            },
            new RunnerPhaseConfig {
                id                  = RunnerPhaseId.TribuIndigena,
                description         = "Entre na floresta amazônica! Pule sobre as raízes e sinta o poder da tribo!",
                sceneName           = "Runner_Tribo",
                startSpeed          = 7.5f,
                acceleration        = 0.07f,
                levelLength         = 115f,
                totalCollectibles   = 20,
                gapFrequency        = 0.2f,
                obstacleFrequency   = 0.3f,
                landmarkNameGar     = "Aldeia do Pajé Paketá",
                landmarkNameCap     = "Aldeia do Pajé Beltrão",
                characterGar        = "Adriano Paketá",
                characterCap        = "Erick Beltrão",
                characterRoleGar    = "Pajé do Boi Garantido",
                characterRoleCap    = "Pajé do Boi Caprichoso",
                quoteGar            = "Os espíritos da floresta abençoam o Garantido! ❤️🌿",
                quoteCap            = "A ancestralidade nos guia, Caprichoso avança! ⭐🌿"
            },
            new RunnerPhaseConfig {
                id                  = RunnerPhaseId.CurralDoBoi,
                description         = "A arena do seu boi! Corra entre as bandeiras e sinta a energia da galera!",
                sceneName           = "Runner_Curral",
                startSpeed          = 8f,
                acceleration        = 0.08f,
                levelLength         = 120f,
                totalCollectibles   = 22,
                gapFrequency        = 0.2f,
                obstacleFrequency   = 0.35f,
                landmarkNameGar     = "Cidade Garantido",
                landmarkNameCap     = "Curral do Caprichoso",
                characterGar        = "João Paulo Faria",
                characterCap        = "Caetano Medeiros",
                characterRoleGar    = "Amo do Boi Garantido",
                characterRoleCap    = "Amo do Boi Caprichoso",
                quoteGar            = "Aqui é a casa do Garantido! Ninguém para esse boi! ❤️",
                quoteCap            = "O Curral do Caprichoso pulsa com a força da estrela! ⭐"
            },
            new RunnerPhaseConfig {
                id                  = RunnerPhaseId.PracaDoisBois,
                description         = "As torcidas se encontram! Corra entre o vermelho e o azul rumo ao Bumbódromo!",
                sceneName           = "Runner_Praca",
                startSpeed          = 9f,
                acceleration        = 0.09f,
                levelLength         = 125f,
                totalCollectibles   = 25,
                gapFrequency        = 0.25f,
                obstacleFrequency   = 0.4f,
                landmarkNameGar     = "Praça — Lado Garantido",
                landmarkNameCap     = "Praça — Lado Caprichoso",
                characterGar        = "Daniela Tapajós",
                characterCap        = "Marcela Marialva",
                characterRoleGar    = "Porta-Estandarte",
                characterRoleCap    = "Porta-Estandarte",
                quoteGar            = "O Estandarte do Garantido ondula para o Bumbódromo! ❤️",
                quoteCap            = "A bandeira do Caprichoso guia a torcida! ⭐"
            },
            new RunnerPhaseConfig {
                id                  = RunnerPhaseId.Bumbodromo,
                description         = "A Arena Olímpica Aylton Papagaio! A noite mais épica do festival começa AGORA!",
                sceneName           = "Runner_Bumbodromo",
                startSpeed          = 10f,
                acceleration        = 0.12f,
                levelLength         = 140f,
                totalCollectibles   = 30,
                gapFrequency        = 0.3f,
                obstacleFrequency   = 0.5f,
                landmarkNameGar     = "Bumbódromo — Lado Garantido ❤️",
                landmarkNameCap     = "Bumbódromo — Lado Caprichoso ⭐",
                characterGar        = "David Assayag",
                characterCap        = "Patrick Araújo",
                characterRoleGar    = "Levantador de Toadas",
                characterRoleCap    = "Levantador de Toadas",
                quoteGar            = "GARANTIDO! O rei do coração domina o Bumbódromo! ❤️🔥",
                quoteCap            = "CAPRICHOSO! A estrela da Amazônia ilumina a arena! ⭐🔥"
            }
        };
    }
}
