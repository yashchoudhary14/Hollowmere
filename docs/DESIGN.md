# Hollowmere — Game Design Doc (v0.2)

> Snapshot of the living design doc in Claude Docs, exported 2026-10-21 (v0.2).
> This file is the source of truth for code in this repo. When the design changes, update this file (or re-export the doc) in the same change.

## Overview

Hollowmere is a 2–6 player co-op horror game. Friends on a road trip get trapped in a walled village, and each of them is given one house to search. Every house hides one part of the ritual that opens the village gate. A match runs about 40 minutes, and the team wins if at least one player gets through the gate and across the fog road.

| Item | Decision |
| --- | --- |
| Title | Hollowmere |
| Genre | Co-op psychological horror, first person |
| Players | 2–6, designed around 4 |
| Match length | 35–45 min |
| Houses | 15 in the village; one allotted per player each match |
| Escape | The Ritual of Passage at the village gate, then the Crossing |
| Setting | Generic rural village, late 1970s |
| Platform | PC (Steam) |
| Engine | Unity 6, URP |
| Multiplayer | Netcode for GameObjects + Unity Relay, Vivox voice |
| Visual target | Phasmophobia-level realism |

**Design pillars** — every feature should serve at least one of these:

1. **Isolation.** Each player spends the night in their own house, with at most one visitor. Contact with friends is always partial: static, distance, dead batteries.
2. **Distrust.** The monster copies voices, faces and clues. Players should regularly ask, "Is that really you?"
3. **Dread over jumpscares.** Long quiet stretches and sounds that build. A scare is earned by minutes of tension.
4. **Shared responsibility.** Every player owns one part of the escape. If someone falls, someone else has to finish their house or bring them back.

**How to use this doc:** numbers and names are starting defaults that playtesting will tune. Open questions are listed in the last section.

## Core loop and match structure

One match is one day and one night in Hollowmere, played in seven phases. By day, players learn which house is theirs. By night, each of them goes in alone to bring back their part of the ritual.

1. **Arrival (1 min).** The friends' car dies at a broken bridge, and the only road leads into Hollowmere. They walk in through the old village gate, which swings shut behind them. Starter kit: flashlight, radio, matches, and a personal keepsake.
2. **Day (12 min).** The notice board holds a sealed letter for each player, addressed by name. It describes that player's house in riddles. Players explore, decode their letters, read the Ritual Book in the chapel and spot the shrine houses. Unmasking the disguised villager is optional.
3. **Dusk (90 s).** The bell tolls 12 times while the villagers walk into the forest. Each player goes into their own house before the last toll.
4. **Night (about 20 min).** Three escalating acts: The Quiet, The Siege, The Hunt. Each player solves their house's puzzle, takes their ritual part and carries it to the altar in the town square. Fallen friends can be revived at shrine houses.
5. **Ritual of Passage (1–2 min).** With every part on the altar, the living stand in the stone circle and keep the candles lit through a 60-second chant while the Hollow attacks. Then the gate opens.
6. **The Crossing (about 2 min).** A fog road leads beyond the gate to the bridge. Players have to walk it without looking back.
7. **Results (1 min).** Who escaped, who died, who was taken on the road, the monster's type, blackout replays, and the final twist.

**Between matches:** players earn Testimony (XP) for escaping, finding lore, identifying the monster and reviving teammates. It unlocks items, cosmetics and, after launch, new villages.

## Setting, lore and premise

Hollowmere is a walled farming village that made a bargain a century ago. Each autumn, the thing in the woods may take travellers, and the village is spared. This year, the travellers are the players.

**Premise (default):** a group of friends on a road trip take a shortcut. Their car dies at a broken bridge, and the only road leads into Hollowmere. Alternative kept on file: each friend receives a letter inviting them to a stranger's will reading in Hollowmere.

**Trapped:** a stone wall and dense forest ring the village. The old gate in the town square is the only way out, and it opens only through the Ritual of Passage.

**The ritual and the houses:** the founders split the gate's ritual into 15 parts and hid one in each house, so no single family could open it alone. Each autumn the village chooses which parts the gate will accept, and gives each guest one house to search. That's why every player gets a letter, and why the needed parts change every match.

**The Hollow:** the entity from the forest. It has no fixed shape and wears what it has taken: voices, faces, the dead. A lit hearth keeps it out. The guests' houses and the shrine houses keep their hearths lit; every other house is a hollow house with a cold hearth.

**The villagers:** friendly by day but evasive, because they are complicit. A few leave clues out of guilt, like the child and the old woman. At dusk they walk into the forest in a line. Every player who dies joins them.

**The bell:** the chapel bell is the bargain's clock. It tolls at dusk and counts the living at midnight. If the gate hasn't opened by the dawn bell, the village keeps its guests.

**Era and tone:** rural folk horror in a generic village in the late 1970s. Rotary phones, CRT televisions, cassette players, oil lamps and walkie-talkies. There's no mobile signal, which explains why nobody can call for help.

**Lore delivery:** letters, diary pages, photographs, gravestones, children's drawings and radio broadcasts. Players collect pages across matches into a journal, and completing it unlocks a secret ending.

## The village map

Hollowmere is about 300 × 300 m inside its wall, with the town square and the gate at the north edge. A night run from a house to the square takes 20–40 seconds of sprinting: short enough to attempt, long enough to be terrifying.

| Zone | By day | By night |
| --- | --- | --- |
| Town square and village gate (north edge) | Arrival; notice board with the letters; the ritual altar and stone circle | The Ritual of Passage; the most exposed ground in the village |
| Bell tower (on the square) | Warning toll, a view over the rooftops | Midnight count and the dawn deadline |
| Post office | Letters, house phone numbers | Dark; the village phone exchange hums |
| Bakery and general store | Villagers, tools, batteries | Locked and dark |
| Chapel and graveyard | The Ritual Book, gravestone clues, mirror shards, handbell | Where the Hollow first appears each night |
| Schoolhouse | Children's drawings, chalk | Desks rearranged; a child's voice |
| Well | Dropped items, lore | Sounds come up from below |
| Fields and scarecrows | Hidden clues among crops | Open ground; scarecrows face different ways |
| Forest edge and wall | Off limits; villagers vanish here at dusk | Where the Hollow comes from; dead players stand with the villagers |
| Houses (15) | The pool of houses to be allotted | One per player, 3–4 shrine houses, the rest hollow |
| Fog road (beyond the gate) | Unseen; the gate is shut | The Crossing, about 200 m to the old bridge |

**House pool:** 15 houses are built. Each match allots one house per player at random, so a 4-player match uses 4 houses. Separately, 3–4 random houses are shrine houses. Because the allotted houses change, so do the routes to the square.

**How the village changes at night:**

- Moonlight and ground fog cut visibility to about 25 m outdoors.
- A fallen tree or cart blocks one random path, so routes must be planned.
- Scarecrows turn to face the nearest player between acts.
- The forest edge creeps closer each act. Trees are literally moved inward, just enough to notice.

## Day phase and clue system

The day lasts 12 minutes, and each player has one main job: find their own house. The team also needs the Ritual Book's instructions and the shrine locations before the bell. Side goals are gathering tools, finding lore and unmasking the disguised villager.

**Your letter.** On arrival, the notice board holds one sealed letter per player, addressed by name. It describes that player's allotted house through 3 riddled traits, drawn from a pool: door colour, number of chimneys, a stopped clock's time, family name, garden contents, window shape. Example: "Yours is the door the colour of a drowned sky, where time stopped at three."

**Decoding.** Clues around the village confirm the traits. A player who confirms 2 of their 3 traits can be sure of their house. Players can help decode each other's letters.

| Clue type | Example | Found at |
| --- | --- | --- |
| Child's drawing | A blue door, a clock stopped at 3 | Schoolhouse, swings |
| Letter | "The Raos never open after dark" | Post office |
| Gravestone | A family name and "they kept the hearth" | Graveyard |
| Villager line | "The house with two chimneys, that one's lucky" | Any villager |
| Radio broadcast | A local notice naming a street | Store radio |
| Photograph | A house with its hearth lit, dated this year | Notice board |

**The Ritual Book.** Chained to the lectern in the chapel. Its pages are randomised every match and show:

- A drawing of each ritual part needed this match, one per allotted house.
- The order the parts must go on the altar.
- The shrine symbol, carved over the doors of the houses where the dead can be brought back.

The book can't be carried, so players memorise or photograph its pages.

**False clues.** One per match (two on Nightmare), planted by the Hollow. It's either a clue pointing toward the wrong house, or a forged Ritual Book page with the wrong order. A wrong order at the altar snuffs every candle and calls the Hollow. Tells: wrong ink colour, a detail that doesn't exist in the village, or a drawing signed by a child buried in the graveyard.

**Villagers.** Six to eight NPCs follow daily routines: baker, postmaster, priest, farmer, old woman, a child. Each has 3–5 lines, and some carry clues. Small wrongness keeps the day uneasy:

- They stop talking when the group approaches together.
- They are sometimes facing you when you turn around.
- They mention players by name before being introduced.

**The disguised villager.** One villager is the Hollow. Its slips depend on the monster variant: repeating words a player said nearby, avoiding sunlight, appearing in two places, waiting to be invited in. A mirror shard held up to it confirms it: no reflection.

- **Correct unmasking:** the Hollow's variant is revealed for the night.
- **Wrong accusation:** the accused villager screams, every player loses 15 sanity, and Act 2 starts 2 minutes early.

**Time signals** so players feel the day running out without a timer on screen:

- Shadows lengthen and light turns gold through the day.
- At 8 min, villagers start walking home.
- At 10 min, birds go silent and the wind rises.
- At 11 min, a single warning toll rings.

**The day is not fully safe.** A figure stands at the forest edge when no one looks directly. Sanity drains slowly when a player is alone in the fields.

## Dusk transition

Dusk lasts 90 seconds, marked by 12 tolls of the chapel bell, one every 7.5 seconds. Each player must be inside their own allotted house when the last toll fades.

**What players see and hear.** The villagers line up and walk silently into the forest. Colour drains from the world over the 90 seconds, and each toll is louder than the last. Doors swing shut and latch on the final toll.

**Rules:**

- **Your house:** each player goes to the house their letter describes. Its hearth is lit, so it starts the night protected.
- **One visitor:** a second player may join you, for a maximum of 2 per house. With a third player inside, the front door won't latch until someone leaves.
- **Locked in:** after the final toll, doors stay latched for all of Act 1. Players inside can open them again from Act 2 onward.
- **Caught outside:** a player outside when the bell ends is hunted immediately. They have about 30 seconds to reach any door, and whoever is inside must decide whether to open it.
- **Hollow houses:** every house that isn't allotted or a shrine has a cold hearth, and the Hollow can walk in freely from Act 2. Tells: clocks run backwards, the family photos are blank, and the radio picks up only a child humming. Hiding in one is a last resort.

## Night phase: three acts

The night lasts about 20 minutes in three acts, each more dangerous than the last. Every player must solve their house's puzzle, take their ritual part and get it onto the altar in the town square before the dawn bell.

**Each player's job:**

1. Solve your house's puzzle (see Houses and puzzles).
2. Take your ritual part. Carrying it means you can't sprint.
3. Carry it to the altar and place it in its slot. A part on the altar is safe; the Hollow can't take it back.

**Act 1 — The Quiet (0–6 min)**

- **The Hollow:** circles the village and never enters. Knocks on doors, footsteps on roofs, a face at a window for a single frame.
- **Houses change:** players notice what's different from the day (see Houses and puzzles).
- **Players:** search the house, find the puzzle and its hints, check the fuse box, set up barricades and close curtains.
- **Radios:** mostly clear.
- **Ends with:** the midnight bell.

**Act 2 — The Siege (6–14 min)**

- **Midnight bell:** tolls once per living player, plus one if an impostor is active.
- **The Hollow:** tests houses. It tries doors and breaks weak windows. It enters if a door is left open, a barricade fails, or noise crosses a threshold.
- **Psychological events begin:** phones, mimic voices, the lying HUD, the extra player.
- **Players:** work through their puzzles, and make the first runs to the square with finished parts.
- **Radios:** about half static.

**Act 3 — The Hunt (14–20 min)**

- **The Hollow:** enters any house freely and targets the loudest player, then the one with the lowest sanity.
- **The dead return:** bodies of dead players are puppeted (see Death, ghosts and reviving).
- **Minute 17:** a village-wide power cut trips every fuse box at once.
- **Players:** get the last parts to the altar, revive the fallen, and start the ritual. Fog thickens.
- **Ends with:** the Ritual of Passage, or the dawn bell, which means the team has lost.

**When someone dies,** their part is still needed. Anyone can enter a dead player's house, and its puzzle stays exactly as they left it. The other option is reviving them at a shrine house (see Death, ghosts and reviving).

**Runs through the village.** Every player has to make the trip to the square at least once. On top of that, one puzzle per match needs an item from another house, allotted or hollow, which forces an extra run.

## Escape: the Ritual of Passage and the Crossing

The team wins if at least one player walks the fog road beyond the gate and reaches the old bridge. It loses if everyone dies, if the dawn bell rings before the gate opens, or if nobody survives the Crossing. There is no vehicle to fix: the escape is a ritual, then a walk.

### The Ritual of Passage

- **Starting it:** every part must be on the altar, in the order shown in the Ritual Book, and every living player must stand inside the stone circle around it. A wrong order snuffs every candle and calls the Hollow.
- **The chant:** a 60-second chant begins. One candle per part stands on the rim of the circle.
- **The attack:** the Hollow circles the square and puts candles out, while the mimic calls from the dark streets. Players relight candles with matches from inside the circle.
- **Resets:** if every candle goes out at once, the chant starts again from zero. If a living player steps out of the circle, the chant pauses until they return.
- **Ghosts help:** each dead player can shield one candle from going out, once.
- **The gate opens** when the chant completes, and the Hollow is thrown back into the forest for a few seconds.

### The Crossing

- Beyond the gate is about 200 m of road through thick fog, ending at the old bridge. Carved into the gate: "Do not look back."
- Players can only walk. Flashlights reach the road ahead and nothing else.
- From behind come footsteps, the voices of dead teammates, and the mimic using a living friend's voice: "Wait for me!"
- Turning to look back (about 120° or more, for half a second) means the Hollow takes you. You vanish, and you join the villagers.
- Walking with a hand on a teammate's shoulder halves sanity loss, but you can't both use flashlights.
- Reaching the bridge means you've escaped.

**Final beat:** at the bridge, the survivors stop and count each other in the fog. There's one more silhouette than there should be. Cut to black.

### Joining the villagers

- Every player who dies joins the villagers. From then on their character is seen standing silently with them: at the forest edge during the night, and along the fog road during the Crossing.
- If the whole team is lost, the final scene shows the next morning. The villagers are back in the square, the players are standing among them, and everyone smiles as a new car pulls up at the gate.

### Results screen

For each player:

- Escaped, died, revived, or taken on the road.
- How, when and where they died.
- How many times the mimic fooled them.
- A replay of what their character did during blackouts.
- The monster's variant, and which villager it was.
- How long each house's puzzle took.

## The monster: the Hollow

The Hollow is a single AI-controlled entity that hunts mainly by sound, learns players' voices, and grows more aggressive each act. It should be heard far more often than it is seen.

**Appearance.** Tall and too thin, wearing pieces of what it has taken: a villager's coat, and for a moment a dead player's face. Mostly seen as a silhouette in fog, eyes at a window, or a hand flat on the glass. It stays very still, then moves in sudden bursts.

| Sense | Range | What it picks up |
| --- | --- | --- |
| Hearing (primary) | 40 m outdoors, 15 m through walls | Voices by mic volume, footsteps, doors, dropped items, phones, radios |
| Sight | 20 m, needs light and line of sight | Flashlights, lit windows, moving players; full darkness hides you beyond 3 m |
| Fear | Whole map | Which player has the lowest sanity; used from Act 3 |

**Abilities:**

- **Voice mimic:** replays short clips it recorded from players, whether spoken near it or sent over a radio.
- **Impersonation:** takes a living player's appearance inside a house (see One too many).
- **Day disguise:** lives as one villager during the day.
- **Puppeting:** wears dead players' bodies in Act 3.
- **Power cut:** can trip one house's fuse box once per act.

**Rules players can learn and use:**

- It cannot cross a lit hearth. Keeping the fire fed makes that room the safest in the house.
- Each barricade plank holds for 10–20 seconds of battering.
- Hiding spots such as wardrobes, beds and crates are safe unless your breathing or heartbeat is heard within 3 m.
- It grabs instead of killing instantly. Teammates have 5 seconds to save you with a camera flash, a burning lantern or the handbell.
- It walks at 1.4 m/s and hunts at 5.5 m/s. Players sprint at 5 m/s for 6 seconds, so you escape by breaking line of sight, not by outrunning it.

**Aggression by act:** Act 1, it never enters a house. Act 2, it enters on noise, open doors or broken barricades. Act 3, it enters freely.

**Behaviour states.** The AI runs a state machine. It roams until it hears or sees something, investigates the source, and tests the house that's nearest to it. If it gets in, it hunts until it grabs someone or loses them. From Act 2, it can switch into a deception (mimic, impostor, puppet) instead of attacking.

```text
Hollow state machine (5 states)

  Roam ──hears or sees something──▶ Investigate ──source is near a house──▶ Test the house
   ▲                                                                          │      ▲ │
   │◀──────────────── barricades hold ────────────────────────────────────────┘      │ ▼
   │                                                                      gets inside │ Deceive (Act 2+)
   │                                                                          ▼       │ mimic, impostor
   └──────────── after a grab, or loses the trail ◀──────────── Hunt ◀── someone falls for it
```

Quiet, well-barricaded houses send it back to roaming; every trick that works pushes it into a hunt.

## Monster variants

Each match secretly picks one of four variants, and all four ship at launch. The base rules stay the same, but each variant changes one sense and adds one rule. Working out which variant you face is the main skill players build over time.

| Variant | What changes | How to tell | Best counter |
| --- | --- | --- | --- |
| The Listener | Hearing doubled, nearly blind | Ignores flashlights; tilts its head at sounds; by day the disguised villager repeats your words | Silence: whisper, crouch, signal with flashlight blinks |
| The Lamplighter | Drawn to light, sees lit areas at 40 m, ignores most sound | Lights flicker before it arrives; candles die near it; by day the villager keeps to the shade | Darkness: kill your own power, move by memory |
| The Guest | Can't enter unless a door is opened from inside or someone says "come in" | Never breaks windows; knocks politely; uses the mimic far more often; by day the villager waits to be invited in | Never open doors, and never say the phrase |
| The Watcher | Moves only when no player is looking at it | Things are always closer than you remember; by day the villager is never seen walking | Keep eyes on it and take turns watching; closing your eyes is deadly |

**Information sources for identifying the variant:**

- Unmasking the disguised villager during the day reveals it outright.
- Each house has one lore note describing "what took them last autumn," which hints at the variant.
- Behaviour in Act 1 gives tells to players who are paying attention.

**Later variants (post-launch ideas):** the Counter, which is obsessed with numbers and corrupts the bell count; and the Hungry, which follows the smell of food and cooking.

## Player systems

Players have no health bar and no weapons. Survival comes from staying quiet, staying in the right places, and saving each other.

| Movement | Speed | Noise radius |
| --- | --- | --- |
| Crouch walk | 1.5 m/s | Silent |
| Walk | 2.5 m/s | 5 m |
| Sprint | 5 m/s for 6 s, then recovers | 25 m |
| Vault or jump | — | 10 m |

**Voice is noise.** The game reads each player's mic volume and turns it into a noise radius: a whisper reaches about 2 m, normal speech 10 m, a shout 30 m.

**Other noise sources:** doors (holding the button opens slowly and quietly; a quick open creaks), dropped items, breaking glass, hammering planks, running on stairs, ringing phones, and radio transmissions at both ends. An optional noise ring on screen shows your current radius on Easy only.

**Condition states:**

- **Healthy:** normal.
- **Injured:** after surviving a grab or a bad fall. Limping, shorter sprints, audible breathing. A bandage fixes it.
- **Grabbed:** a 5-second window for a teammate to save you. A saved player becomes Injured and drops what they were holding.
- **Dead:** becomes a ghost.

**Inventory:** 3 hand slots plus 1 ritual-part slot, and a keepsake that can't be dropped. Carrying a ritual part means no sprinting, so carriers need an escort.

**Physical interactions:** drawers and doors move with the mouse, notes can be turned over to find writing on the back, and barricades are built by holding a plank to a frame and hammering.

**Holding your breath:** in a hiding spot, holding a key holds your breath for up to 8 seconds. This hides your breathing and heartbeat from the Hollow. Holding too long ends with a loud gasp.

**Pulse check:** hold the interact key on a teammate to read their sanity as steady, fast, racing or erratic. This is the only way to see someone else's state.

## Sanity

Each player has a hidden sanity value from 100 to 0. It never appears as a number; players read it from their own body and from how the world starts behaving.

**Core rule:** hallucinations never kill you directly, but they can trick you into doing something that does.

| Drains sanity | Amount |
| --- | --- |
| Being in darkness | −1 every 10 s |
| Being alone (no teammate within 8 m) | −1 every 10 s, stacks with darkness |
| Seeing the Hollow | −10 per sighting |
| Seeing a body | −15, once per body |
| A psychological event aimed at you | −5 to −10 |
| Act multiplier | ×1.5 in Act 2, ×2 in Act 3 |

| Restores sanity | Amount |
| --- | --- |
| Teammate within 3 m | +1 every 10 s |
| Near a lit candle or hearth | +1 every 10 s |
| Closing your eyes | +2 per second, but you're blind |
| Chamomile tea or sedatives | +25 |

| Range | State | What happens |
| --- | --- | --- |
| 100–75 | Steady | Normal play, quiet ambient sounds |
| 74–50 | Uneasy | Phantom footsteps, shadows at the edge of vision, you hear your own heartbeat |
| 49–25 | Frayed | Fake radio voices, objects in the wrong place, the HUD starts lying, the Hollow hears your heartbeat within 5 m |
| 24–1 | Breaking | Blackouts, impostor events target you, you hear your own voice, the Hollow prefers you in Act 3 |
| 0 | Gone | A forced blackout; you wake in the darkest room of the house at 15 sanity, missing one item |

**Visual signs scale with sanity:** film grain, a narrowing vignette, colour slowly draining, and slight chromatic aberration below 25.

## Psychological horror events

These events are what make Hollowmere different from other co-op horror games. An event director spaces them out based on group tension, so no two matches play the same. Each event below lists when it fires, what happens, how to spot it, and how to beat it.

1. **One too many**
   - **When:** Act 2 or later, once per match, in a house with 2 or more players.
   - **What happens:** a copy of a teammate walks in from another room, with their name, model and recorded voice. It suggests splitting up or checking the cellar.
   - **Tells:** no reflection, silent footsteps, never carries items, never uses the radio. The real teammate may be talking on the radio from elsewhere at the same moment.
   - **Counterplay:** a mirror shard, a question only the real player could answer, or the handbell, which makes it flinch.
   - **Outcome:** ignored, it leads someone alone into a dark room. Exposed, it flees and the house gets 60 seconds of calm.
2. **Blackout**
   - **When:** sanity below 25.
   - **What happens:** your screen cuts to black for 4–8 seconds and you wake in another room. Meanwhile the AI controls your character and does one thing: opens a window, hides an item, removes a barricade, writes on a wall, or just stands staring at a teammate.
   - **Tells:** your teammates see it happen live. You only see the aftermath.
   - **Counterplay:** keep your sanity up. A teammate can shake you awake early.
   - **Build note:** the server takes over the character temporarily and records the action for the results screen.
3. **The lying HUD**
   - **When:** sanity below 50.
   - **What happens:** the inventory shows an item you don't have, the player list shows a dead friend as alive, a fake "Player joined" message appears, or the objective quietly changes to "Open the front door."
   - **Tells:** every lie is drawn in a very slightly wrong font. Observant players learn it.
   - **Build note:** runs on that player's machine only, so it needs no networking.
4. **The phone**
   - **When:** Act 2 or later. Every house has a rotary phone, and house numbers are found during the day.
   - **What happens:** players can call each other on a clearer line than radios. The Hollow also calls using mimic clips, and sometimes the caller is your own voice from earlier in the match.
   - **Counterplay:** a villager suggests agreeing on a code word during the day. The mimic can only repeat what it has heard, so a code word said near the disguised villager is compromised.
   - **Cost:** an unanswered phone rings loudly and draws the Hollow.
5. **The mimic at the door**
   - **When:** Act 2 or later; most common with the Guest.
   - **What happens:** a knock, then a friend's real recorded voice: "It's me, let me in."
   - **Counterplay:** check on the radio or phone first, or look through the window, if you dare.
6. **The dead come back wrong**
   - **When:** 2–4 minutes after a death, in Act 3.
   - **What happens:** the body vanishes. Later the dead player's character stands outside their friends' window, knocking and repeating things they said. The dead player watches as a ghost.
   - **Counterplay:** it can't enter unless let in, but each sighting costs sanity.
7. **Voices in the static**
   - **What happens:** dead players can speak to the living only through radio static. The mimic uses the same channel, so the living can never be sure which voice is really their friend.
8. **The bell count**
   - **When:** midnight.
   - **What happens:** the bell tolls once per living player. An extra toll means an impostor is active or about to be.
9. **A house with your name**
   - **When:** once per match, in a random hollow house.
   - **What happens:** one player's real Steam name is on the mailbox. Inside are letters addressed to them describing things they did during the day, built from the match's own event log: "We saw you at the well, just after four."
   - **Purpose:** pure dread. Nothing in it is dangerous.
10. **Closing your eyes**
    - **What happens:** any player can hold a key to close their eyes and restore sanity. When they open them, objects have moved, and once in a while something is standing much closer.
    - **Danger:** against the Watcher, this is deadly.
11. **The planted clue**
    - **When:** day, plus one follow-up at night.
    - **What happens:** a false clue leads to a hollow house. At night, a note in a real house says the key is in a room where nothing is waiting except the Hollow.
12. **The late mirror**
    - **What happens:** mirrors show the room half a second late. At low sanity your reflection turns its head on its own. Once per match, the reflection stays standing after you walk away.
13. **The family photo**
    - **What happens:** a photo gains an extra person who gets closer to the camera each act. In Act 3, the face is a player's.
14. **The wrong room**
    - **What happens:** a door briefly opens onto a room from another house, the one you'll have to run to later. Close it and open it again, and it's normal.

**Event director rules:**

- After a big scare, guarantee 60–90 seconds of calm.
- Never fire two events at the same player within 2 minutes.
- Weight events toward the player with the lowest sanity, but don't let one player get all of them.

## Communication

Players can always talk, but never safely and never with certainty. Every channel has a range, a cost, and a way the Hollow can abuse it.

| Channel | Reach | Quality | Risk |
| --- | --- | --- | --- |
| Proximity voice | About 20 m, muffled by walls and doors | Clear | The Hollow hears by volume |
| Walkie-talkie | Whole village | Static grows each act | Noise at both ends; battery; the mimic can cut in |
| House phone | House to house, by number | Clear | Loud ringing; the mimic can call |
| Ghost static | Dead to living, through radios | Heavily distorted | Sounds identical to the mimic |
| Flashlight blinks | Line of sight between windows | Silent | Draws the Lamplighter |
| Chalk on walls | Same house, later readers | Silent | The Hollow can alter messages |

**Radio details:**

- Push to talk. Battery is shown on a needle gauge and drains only while transmitting, about 6 minutes in total.
- Static gets louder when the Hollow is within 10 m of either radio. This makes the radio a rough monster detector, which players will learn to use.
- Starter kit: one radio per player.

**Voice recording for the mimic:**

- The game records short 2–4 second clips of each player's voice during a match, and deletes them when the match ends.
- Players are told about this on first launch. There's an opt-out in settings, which makes the mimic use generic villager voices for that player.
- The Hollow only uses clips from words that were spoken near it or over a radio. This makes being quiet around the disguised villager a real tactic.

## Items and tools

Every item helps with one problem and creates another. There are no weapons, only ways to see, hide, signal and buy time.

| Item | What it does | Where | The catch |
| --- | --- | --- | --- |
| Flashlight | 15 m cone of light | Starter kit, one each | 8 min of battery; draws the Lamplighter |
| Walkie-talkie | Village-wide voice | Starter kit, one each | Battery, noise, mimic |
| Matches and candles | Light and slow sanity recovery | Every house | 3 min burn; blown out by drafts and opened doors |
| Oil lantern | 360° light; saves a grabbed teammate | General store (day) | Heavy; only one per match |
| Hammer and planks | Barricade a door or window | Houses, store | Hammering is very loud |
| Mirror shard | Reveals impostors and the disguised villager | Chapel, some bathrooms | Cracks after 3 uses |
| Instant camera | Flash saves a grabbed teammate; photos show the Hollow even when you can't | Post office | 5 shots |
| Handbell | Stuns the Hollow for 3 s; makes impostors flinch | Chapel | Heard across the village |
| Fuse | Restores a house's power | Cellars, attics | Few per house |
| Batteries | Refill a flashlight or radio | Store, drawers | Limited |
| Bandage | Injured to Healthy | Bathrooms | 5 s to apply, while vulnerable |
| Chamomile tea | +25 sanity | Kitchens | Needs the stove: light and noise |
| Chalk | Silent messages on walls | Schoolhouse | The Hollow can change what you wrote |

**Ritual parts** go in their own slot and stop you sprinting. There are 15, one hidden in each house (see Houses and puzzles), and each match needs one per player.

**Keepsake:** every player starts with a personal keepsake, such as a photo, a watch or a ring. It drops where they die, and it's needed to revive them.

**Day tip:** players who photograph each house's family photo during the day will have an advantage later (see Rao House).

## Houses and puzzles

Hollowmere has 15 houses, and each hides one ritual part behind its own puzzle. They are all built from one shared template, then dressed differently. Puzzles change every match so players can't simply memorise the answers.

**Shared template:**

- Two floors plus a cellar or attic, 6–8 rooms: hall, kitchen with hearth, living room with phone, two bedrooms, bathroom with mirror, cellar or attic with fuse box.
- Front and back doors, 6–8 windows. Two windows are weak and act as break-in points.
- Always present: hearth, fuse box, phone, mirror, family photo, wardrobes, beds, radio or TV.
- 4–6 hiding spots: wardrobes, under beds, cellar crates, behind the bathtub curtain. Each has a narrow peek view.

**Night changes** (2–3 picked per house each match):

- The family photo has an extra person.
- A door now opens onto a brick wall.
- Furniture has moved since the day.
- A hallway has one more door than it did.
- Every clock has stopped at the same time.
- Writing on a wall in a player's name.

### The 15 houses

| House | Ritual part | Example puzzle |
| --- | --- | --- |
| Blue Door House | Iron gate key | Set the grandfather clock to the time in the child's drawing |
| Rao House | Family seal ring | Lockbox code from the family photos as they looked by day |
| Widow's House | Hearth ember | Feed the right keepsake to the fire to make an ember |
| Schoolmaster's House | Bell clapper | Spell the hidden word on the slate from scattered chalk letters |
| Weaver's House | Spool of red thread | Rethread the loom to match a pattern hidden in the house |
| Doctor's House | Glass eye | Match medicine bottles to patient records to open the cabinet |
| Twins' House | Silver locket | Spot the differences between the house's two mirrored halves |
| Organist's House | Organ pipe | Play the village lullaby on the pump organ, loudly |
| Butcher's House | Bone whistle | Balance the scale to the total written in the ledger |
| Miller's House | Last-harvest sheaf | Restart the mill by linking its gears in the right order |
| Photographer's House | Glass negative | Develop a photo in the darkroom to reveal a code |
| Priest's House | Altar cloth | Turn the hymn numbers on the board into a cupboard code |
| Postmaster's House | Sealed letter | Sort letters into pigeonholes using address clues |
| Farmer's House | Straw heart | Match the scarecrows' positions to the barn drawing |
| Dollmaker's House | Porcelain hand | Place the dolls in the dollhouse where people stand in the real house |

### Keeping puzzles fresh

- Each house has 2–3 puzzle variants, and one is picked per match.
- Every variant randomises its answer: codes, orders, which object is the key, and where the hints are hidden.
- Once per match, one puzzle needs an item from another house, which forces an extra run.
- The full puzzle catalogue gets designed once the core loop is locked.

### Shrine houses

- Each match, 3–4 random houses are shrine houses, where the dead can be revived (see Death, ghosts and reviving).
- A shrine symbol is carved over their doors, and the Ritual Book shows it.
- A shrine house can also be someone's allotted house.
- Shrine hearths stay lit, so they are protected like allotted houses.

### Example: Blue Door House (the clockmaker's)

- **Signature:** blue door, a clock stopped at 3:00, two chimneys.
- **Ritual part:** the iron gate key, locked inside the grandfather clock.
- **Puzzle:** find the winding key hidden in the house, then set the clock to the time shown in the child's drawing. The time changes every match.
- **Twist:** when it opens, the clock chimes 12 times, loudly. Barricade first.

### Example: Rao House (the family that never opened the door)

- **Signature:** "Rao" on the gate, shuttered windows, a vegetable garden.
- **Ritual part:** the family seal ring, in a lockbox in the cellar.
- **Puzzle:** restore power at the fuse box to light the cellar. Writing on the cellar wall explains the code: the number of people in certain family photos upstairs. Which photos count changes every match.
- **Twist:** at night the photos have extra people, so counting them now gives the wrong code. Players need to remember the photos from the day, or have photographed them.

### Example: Widow's House

- **Signature:** black ribbon on the door, a single lit window, a rocking chair on the porch.
- **Ritual part:** the hearth ember, carried out in the oil lantern.
- **Puzzle:** the fire burns low and grey. It only gets hot enough to make an ember when the widow's wedding photograph is fed to it. In the cross-house variant, that photograph is in another house.
- **Twist:** taking the ember lets the fire die, so the house loses its protection. Whoever carries the ember out gives up their own safety.

## Death, ghosts and reviving

Dead players keep playing as ghosts, and they can be brought back once through the Rite of Return at a shrine house. Reviving costs the team time, which is the trade-off.

**Dying.** A short first-person sequence: you're dragged into darkness, then wake as a ghost standing over your own body. Your keepsake drops beside it. Living players who find the body lose 15 sanity.

**Ghost powers** use an energy bar that recharges slowly:

- Flicker a light, to warn someone.
- Knock once on a surface, to signal.
- Speak through radio static, heavily distorted.
- Write up to five letters on a fogged mirror, once per act. The Hollow can also write on mirrors.
- See the Hollow faintly through walls at all times. This is valuable information, if you can get it across.
- Shield one ritual candle from going out during the chant, once.

**Ghost limits:** ghosts can't open doors, carry items or speak in normal voice to the living. They hear living players near them and can talk freely with other ghosts.

### The Rite of Return

1. Pick up the dead player's keepsake from where they fell.
2. Carry it to any shrine house and place it on the shrine.
3. Light the shrine candle and hold the interact key for 30 seconds. The chant is loud, reaching about 20 m.
4. The dead player wakes at the shrine: Injured, at 25 sanity, with empty hands.

**Limits:** each player can be revived once, and each shrine works once per match.

**Their house:** a revived player picks up their puzzle where they left it. If someone else already finished it, they're free to help elsewhere.

### The puppet

In Act 3, 2–4 minutes after a death, the Hollow takes the body. You watch it walk to your friends' house and use your voice to ask to be let in. You can warn them through the static, but they can't tell your warning from the mimic.

If you're revived after your body was taken, the puppet keeps walking. You can meet yourself at a window.

### Joining the villagers

While dead, your character also appears among the villagers at the forest edge, standing silently with its face turned away. If the team wins and you weren't revived, the results screen marks you as freed.

## Audio and atmosphere

Sound carries most of the horror, so it should get more development time than the monster's animations. The visual style supports it: warm and almost too pleasant by day, cold and close by night.

**Audio principles:**

- **Silence is a tool.** Act 1 has long stretches of only room tone, clock ticks and wind.
- **3D positional audio** (Steam Audio) so players can tell which floor or which side of the roof the footsteps are on.
- **Every scare has a build-up:** a sound 10–30 seconds before the event.
- **The Hollow's signature:** wet breathing, joints clicking, and a soft hum of the lullaby the village children sing during the day. Players hear it as innocent by day and dreadful by night. It should be an original composition.
- **Only in-world music:** radios, a music box, the chapel organ. No orchestral horror stings; the bell is the only sting.
- **Your own body:** breathing and heartbeat get louder as sanity drops.

**Visual style:**

- **Day:** golden autumn light, slightly oversaturated, birds, laundry moving on lines.
- **Dusk:** orange fading to grey over 90 seconds as colour drains.
- **Night:** moonlight, volumetric fog and flashlight cones. House interiors are darker than outside.
- **Camera:** subtle head bob; film grain, vignette and colour loss scale with sanity.
- **Era props:** rotary phones, CRT televisions, cassette players, oil lamps, walkie-talkies.

## Replayability, difficulty and progression

Replay value comes from randomising the parts players learn: whose house is whose, which parts the gate wants, how each puzzle is solved, and what the monster is. The village stays familiar while each match stays unpredictable.

**Randomised every match:**

- Which houses are allotted, and to which player.
- The ritual parts needed and their order on the altar.
- Which 3–4 houses are shrines.
- Each house's puzzle variant and its answer.
- Which puzzle needs an item from another house.
- The wording of each player's letter, and the clue placement.
- The planted false clue.
- Which villager is the Hollow, and its variant.
- Which psychological events fire, and when.
- The night changes inside each house.

| Setting | Easy | Normal | Nightmare |
| --- | --- | --- | --- |
| Day length | 15 min | 12 min | 9 min |
| Noise ring on screen | Shown | Hidden | Hidden |
| Hollow hunting speed | 4.5 m/s | 5.5 m/s | 6.2 m/s |
| Sanity drain | ×0.7 | ×1 | ×1.4 |
| False clues | 0 | 1 | 2 |
| Unmasking reveals variant | Yes | Yes | No |
| Ghost powers | All | All | Radio only |

**Progression:**

- **Testimony (XP)** for escaping, collecting lore, identifying the variant, reviving and saving teammates.
- **Unlocks:** the instant camera and handbell (locked at first, to keep early matches simple), cosmetics from village wardrobes, and new villages after launch. All four monster variants are in from the start.
- **The journal:** collected lore pages. Completing it reveals the village's full story and unlocks a secret ending.

## Sample match timeline

A four-player match on Normal runs about 38 minutes. A is allotted the Blue Door House, B the Organist's House, C the Rao House and D the Widow's House. The shrines are the Priest's, Weaver's and Doctor's houses, and the monster is the Listener.

| Clock | Phase | What happens |
| --- | --- | --- |
| 0:00 | Arrival | The car dies at the bridge; the friends walk in and the gate shuts behind them |
| 1:00 | Day | Four sealed letters on the notice board, one per name |
| 3:00 | Day | A's letter mentions a drowned-sky door and a clock stopped at three; a child's drawing points to the Blue Door House |
| 5:00 | Day | The Ritual Book shows a key, an organ pipe, a seal ring and an ember, in that order, plus the shrine symbol |
| 6:30 | Day | The baker asks C about his brother, whom C only mentioned on voice chat nearby |
| 8:00 | Day | A mirror shard shows the baker has no reflection, revealing the Listener |
| 10:00 | Day | B and D confirm their houses; the shrine symbol is spotted on three doors |
| 12:00 | Day | Warning toll; C photographs the Rao family photos |
| 13:00 | Dusk | Twelve tolls; each player walks into their own house alone |
| 14:30 | Act 1 | Doors latch; each house looks slightly wrong |
| 17:00 | Act 1 | A finds the winding key; knocking and footsteps on roofs |
| 20:30 | Act 2 | Midnight bell tolls four times |
| 22:00 | Act 2 | B plays the lullaby on the organ; the noise brings the Hollow, and B hides in a wardrobe, holding his breath |
| 24:00 | Act 2 | C's phone rings with D's voice; the real D on the radio says she never called |
| 25:30 | Act 2 | A sets the clock, survives the chimes, and runs the gate key to the altar |
| 28:30 | Act 3 | B is caught carrying the organ pipe and dies in the street |
| 30:00 | Act 3 | A picks up B's keepsake and pipe and performs the Rite of Return at the Priest's House |
| 31:30 | Act 3 | Village-wide power cut; C uses the day-time photos to open the lockbox in the dark |
| 32:30 | Act 3 | Revived B sees his own puppet standing at a window |
| 33:00 | Ritual | All four parts on the altar; the chant starts; candles go out twice and are relit |
| 34:00 | Ritual | The gate opens |
| 34:30 | Crossing | B hears his own voice behind him, turns, and is taken |
| 36:00 | Crossing | A, C and D reach the bridge and count four silhouettes in the fog |
| 37:00 | Results | B is shown standing among the villagers |

## Decisions and open questions

Your checklist answers and the design conversation are now built into the sections above. The table records what's settled; the list below holds the next questions.

| Topic | Decision |
| --- | --- |
| Players | 2–6, designed around 4 |
| Premise | Friends on a road trip (inherited letters kept as an alternative) |
| Setting | Generic rural village, late 1970s |
| Houses | 15 in the pool; one allotted per player each match |
| Per house | Max 2 players: the owner plus one visitor |
| Ritual | One part per allotted house; recipe and order random each match |
| Ending | Ritual of Passage at the gate, then the Crossing; no bus, no bell-ringer sacrifice |
| If someone dies | Another player finishes their house, or revives them |
| Reviving | Rite of Return at 3–4 shrine houses, using the keepsake |
| Puzzles | Randomised variants and answers; full catalogue designed later |
| Dead players | Join the villagers within the match; no carry-over to the next match |
| Voice mimic | On, with an opt-out |
| Radios | One per player |
| Monster variants | All four at launch |
| Title | Hollowmere |

- [ ] **Ritual circle:** must every living player stand in it, or is one enough?
- [ ] **Shrine houses:** random 3–4 each match, or always the same houses?
- [ ] **House parts:** does each house always hide the same part, or are parts shuffled between houses too?
- [ ] **Revive limit:** once per player and once per shrine, or more generous?
- [ ] **The Crossing:** does looking back kill you, or drag you back to the gate with heavy sanity loss?
- [ ] **Starting the ritual:** any time once all parts are placed, or only during Act 3?
- [ ] **Day phase:** keep the full 12-minute day, or shorten it now that each player only needs to find one house?
