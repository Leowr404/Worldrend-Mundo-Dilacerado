# GUIA — Worldrend: Mundo Dilacerado

Referência rápida pra não se perder. Consulta antes de criar item/quest/inimigo.

---

## 🧩 MANAGERS (não podem faltar na cena de gameplay — TestPlace)

| Manager | O que faz | Campos que NÃO podem faltar |
|---------|-----------|------------------------------|
| **QuestManager** | Controla quests | `allQuests` = TODAS as quests do jogo |
| **SaveManager** | Salva/carrega | `inventorySaver`, `player`, `cameraTransform` |
| **EquipmentManager** | Equipar/bônus | `equipmentSlots` (5), `player`, `weaponHolder`, `inventorySlots` |
| **ItemDatabase** | Lista de itens | `items` = TODOS os itens do jogo |
| **InventorySaver** | Salva inventário | `slots` (os slots do inventário) |
| **ItemCollector** | Coletar no chão | `inventorySlots` |
| **EconomyManager** | Moeda | — |
| **UiManager** | HUD/diálogo | painéis ligados |
| **InputManager** | Controles | — |
| **DeathScreenUI** | Tela de morte | `deathPanel` |
| **AudioManager** | Som | — |

---

## ➕ CRIAR ITEM NOVO (consumível ou material)

1. Botão direito no Project → **Create → Game → Item**
2. Preenche:
   - `Item Name`
   - `Item Id` → **ÚNICO** (nenhum outro item com o mesmo número!)
   - `Item Sprite` → ícone do inventário
   - `Item Type` → Material ou Consumable
   - Consumível: `Health Restore` / `Stamina Restore`
   - `Is Stackable` → consumível geralmente SIM
3. ⚠️ **ADICIONA no ItemDatabase → lista `Items`** (senão SOME no save/load!)
4. Pra coletar no chão: cria um prefab com o modelo + **Collider (Is Trigger)** + componente **ObjectType** (arrasta o Objects no campo)

---

## ⚔️ CRIAR ARMA / ARMADURA (equipável)

1. **Create → Game → Item**
2. Preenche:
   - `Item Id` → **ÚNICO**
   - `Item Type` → **Equipment**
   - `Equip Slot` → Helmet / Chest / Glove / Boot / Weapon
   - `Min/Max Defense` e/ou `Min/Max Attack` → **> 0** (senão não dá bônus nenhum)
   - `Is Stackable` → **FALSE** (senão empilha e perde a defesa individual)
   - Arma: `World Model` → prefab do modelo 3D (aparece na mão)
3. ⚠️ **ADICIONA no ItemDatabase → lista `Items`**
4. Coletável no chão: prefab + Collider trigger + ObjectType

---

## 📜 CRIAR QUEST NOVA

**1. Cria a quest:** botão direito no Project → **Create → RPG → Quest**

**2. Preenche cada campo:**

**Identificação:**
- `Quest Name` → nome da quest (aparece no HUD/journal)
- `Description` → descrição do que fazer

**Objetivo (Objective):**
- `Type` → o que precisa fazer:
  - **Kill** → matar inimigos
  - **Collect** → coletar itens
  - **ExploreArea** → chegar numa área
  - **TalkToNPC** → falar com um NPC
- `Target ID` → ⚠️ **o ID do alvo, tem que bater EXATAMENTE** com:
  - Kill → o `enemyID` do **EnemyStats** do inimigo (ex: `"goblin"`)
  - Collect → o `itemID` do **CollectibleItem** (ex: `"herb"`)
  - ExploreArea → o `areaID` do **ExploreAreaTrigger** (ex: `"caverna"`)
  - TalkToNPC → o `npcID` do **DialogueTrigger** (ex: `"velho"`)
- `Description` → texto que aparece no HUD (ex: "Matar 5 Goblins")
- `Required Amount` → quantos precisa (Kill/Collect). Pra Explore/Talk = **1**
- `Current Amount` → deixa **0** (é o progresso em runtime)

**Entrega:**
- `Delivery NPCID` → ⚠️ ID do NPC onde entrega a quest. **Tem que bater com o `Npc ID` do DialogueTrigger** desse NPC
- `Auto Complete` → **marca** se a quest conclui sozinha ao terminar o objetivo (sem voltar ao NPC). Desmarcado = precisa entregar no NPC

**Recompensas:**
- `Xp Reward` → XP ao concluir
- `Money Reward` → dinheiro ao concluir
- `Gives Money` → **marca** se dá dinheiro (desmarcado = não dá money mesmo com valor acima)

**3.** ⚠️ **ADICIONA no QuestManager → lista `allQuests`** (senão NÃO SALVA e dá pra refazer!)

**4.** (Opcional) Parede de bloqueio: ver seção 🧱 Barreiras de Quest

### Fluxo de uma quest
1. Player aceita (pelo diálogo do NPC que tem a quest)
2. Faz o objetivo (mata/coleta/explora/fala) → progresso sobe sozinho
3. Objetivo completo → "Volte ao NPC"
4. Fala com o NPC de entrega (`Delivery NPCID`) → ganha XP/dinheiro → quest concluída

> ⚠️ Os **2 IDs que mais quebram quest:** o `Target ID` (tem que bater com o ID do inimigo/item/área/NPC) e o `Delivery NPCID` (tem que bater com o NPC de entrega). Se um deles não bater, a quest nunca progride ou nunca conclui.

---

## 👹 CRIAR INIMIGO / SPAWNER

1. Prefab do inimigo precisa ter: **NavMeshAgent**, **EnemyStats**, **Animator** com o FSM
2. EnemyStats → configura stats e a lista **Drops** (prefab + chance + quantidade) se quiser drop
3. Spawner: Empty → componente **EnemySpawner**:
   - `Enemy Prefab`, `Max Enemies`, `Spawn Radius`, `Respawn Delay`
   - `Waypoint Area` → o objeto pai dos waypoints
4. WaypointArea: Empty com vários Empties filhos (cada filho = ponto de patrulha)
5. ⚠️ **NavMesh tem que estar BAKEADO** (Window → AI → Navigation → Bake), e o Spawn Radius sobre o NavMesh

---

## 💬 CRIAR DIÁLOGO / NPC QUE FALA

**1. Cria o diálogo** (ScriptableObject):
- Botão direito no Project → **Create → RPG → Dialogue**
- Preenche:
  - `Npc Name` → nome que aparece na caixa
  - `Lines` → falas genéricas (quando o NPC não tem quest)
  - **Falas da quest** (se vincular uma quest):
    - `Before Quest` → fala antes de aceitar a quest
    - `During Quest` → fala enquanto a quest está em andamento
    - `After Quest` → fala depois de entregar a quest
  - `Quest` → (opcional) a quest que esse NPC dá
  - `Options` → opções de resposta (opcional)

**2. Configura o NPC na cena** (componente **DialogueTrigger**):
- `Dialogue` → arrasta o DialogueData que criou
- `Npc ID` → identificador do NPC (texto, ex: `"ferreiro"`)
- `Interaction Icon` → (opcional) ícone "aperte E" que aparece quando o player chega perto
- O NPC precisa de um **Collider com Is Trigger = ON** (detecta o player chegando)

**Como funciona:**
- Player entra no trigger → ícone aparece
- Aperta **Interagir** (E) → abre o diálogo
- Se o NPC tem quest: mostra Before/During/After conforme o estado da quest
- Entrega a quest automaticamente se o player completou o objetivo

⚠️ **O `Npc ID` do DialogueTrigger precisa bater com o `deliveryNPCID` da Quest** — é assim que a quest sabe que foi entregue nesse NPC. Se não baterem, a quest nunca conclui.

---

## 🧱 BARREIRAS DE QUEST (parede invisível que abre ao completar quest)

Trava uma passagem até o player completar uma quest específica.

1. GameObject → **3D Object → Cube**
2. Estica/posiciona o Cube pra **cobrir a passagem**
3. **MeshRenderer** → desmarca (vira invisível; o collider continua barrando)
4. **BoxCollider** → ligado, **Is Trigger = OFF** (sólido, empurra o player)
5. Add Component → **QuestBarrier**
6. `Required Quest` → arrasta a quest que **destrava** a passagem

**Como funciona:**
- Não desativa o GameObject — liga/desliga só o **Collider**
- Quest incompleta → collider ON (bloqueia) | Quest completa → collider OFF (passa)
- Sobrevive a save/load: ao carregar, re-checa o estado real da quest
- Cada passagem = um Cube com seu próprio QuestBarrier

⚠️ A quest do `Required Quest` precisa estar na lista **`allQuests`** do QuestManager.

---

## 💀 O QUE QUEBRA (armadilhas comuns)

| Sintoma | Causa | Fix |
|---------|-------|-----|
| Quest dá pra refazer / não salva | `allQuests` vazio no QuestManager | Arrasta todas as quests no `allQuests` |
| Item some no save/load | Item não está no ItemDatabase | Adiciona o item na lista `Items` |
| Load carrega item errado | `itemId` duplicado | Cada item com Id único |
| Inimigo não anda | NavMesh não bakeado | Bake o NavMesh |
| Equipável não dá defesa | `Is Stackable` = true OU Min/Max = 0 | Stackable false + Min/Max > 0 |
| Drop de drag não funciona | Fundo full-screen com Raycast Target ON, ou Canvas sem Graphic Raycaster | Tira Raycast Target do fundo; põe Graphic Raycaster no Canvas |
| Item não solta no slot | `itemIcon` (filho) com Raycast Target ON | Raycast Target só no fundo do slot, OFF no ícone |
| Espada não aparece na mão | Falta WeaponHolder ou Hand Socket | WeaponHolder no Player + Socket no osso da mão |

---

## 💾 O QUE O SAVE GUARDA

- Posição do player + rotação da câmera
- **Vida** (PlayerStats.currentHealth), stats, XP, level, pontos
- Moeda
- Hora do mundo (Skyboxspin)
- **Quests** (completas + progresso das ativas) → precisa `allQuests`
- **Inventário** (com defesa/ataque rolados) → precisa ItemDatabase
- **Equipamento** equipado → precisa ItemDatabase
- Tempo de jogo

**Auto-save:** automático ao **completar uma quest** (slot 0).
**Continue (menu):** carrega o slot 0; o botão só aparece se existe save.

> ⚠️ Regra de ouro do save: **todo item precisa estar no ItemDatabase** e **toda quest precisa estar no allQuests**. Esses dois são a causa de 90% dos bugs de save/load.

---

## 🎬 CENAS

- **MainMenu** → menu (Play = novo jogo, Continue = carrega save)
- **Loading** → tela de carregamento (intermediária)
- **TestPlace** → gameplay principal

Fluxo: MainMenu → Loading → TestPlace
