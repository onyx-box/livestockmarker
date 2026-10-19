# Livestock Marker roadmap

This document describes **planned functionality**, not features already included in 0.1.0-pre.1.

## Next: improve the first release
- Remove mesh debugging and noisy logs from production; keep controlled diagnostics behind an explicit debug flag.
- Verify packed-ZIP server restart persistence, permissions and compatibility with additional mods.
- Replace brittle fixed growth behavior indices with a safer integration mechanism.
- Update vulnerable dependencies in the CakeBuild toolchain where compatible.
- Improve localized command messages and expand visible coat support beyond adult Valais goats.

## Animal names and text labels
- Add a configurable display name (for example **Bella**, **Luna**, **Bruno**) independent from the technical entity code.
- Show the name in the tooltip, and optionally over the animal at close distance.
- Set, update and clear names via commands and later an in-game UI.
- Keep a stable animal-specific identity across life stages, rather than relying solely on numeric `EntityId` (which can change when an entity is replaced).
- Validate lengths, Unicode, and safe display formatting; consider moderation on public servers.

Suggested commands (not implemented): `/lm name Bella`, `/lm name clear`.

## Change history / audit log
- Record who changed a mark, name or ownership, with server timestamp, action, old value and new value.
- Use the authenticated **stable player UID**, plus an optional display name snapshot; do not trust the client to submit the actor identity.
- Define server-side retention, storage limits, access permissions and privacy policy.
- Preserve meaningful history through animal growth; avoid unbounded attributes or oversized save data.

Possible record: `animalUid, playerUid, action, oldValue, newValue, serverTime`.

## Ownership and access control
- Distinguish a cosmetic **owner label** from enforceable **permissions**.
- Owner identity should be a stable player UID; show the current player name as a convenience.
- Proposed default: unowned animals can be marked by any permitted player; owners can modify their own animals; operators have a controlled override.
- Consider shared ownership, server/admin ownership reassignment, abandoned animals, and griefing prevention.
- Enforce every ownership check on the **server**, including marking, clearing, renaming and ownership transfer, not just the UI.
- Define who can claim an animal: claiming any unowned animal may be unsafe on public servers. Prefer an explicit claim mechanism and configurable server policy.
- Carry ownership securely when a young entity grows to adulthood.
- Avoid leaking private player identifiers in public tooltips or client packets.

Possible commands (not implemented): `/lm claim`, `/lm owner`, `/lm transfer <player>`.

## More visual markings
- More livestock species, ages, patterns and body zones.
- Contrast-aware markings for white/black coats.
- Texture generation and caching shared between animals with identical variants.

## Long term: brush painting
- Paint directly on the creature under the cursor, with actual face hit testing and UV mapping.
- Store compact, replayable paint strokes or appropriately compressed masks.
- Server-authoritative updates, collision/brush validation, network synchronization, undo, and limits against abuse.
- Support body rotation and animation and avoid modifying shared textures for all animals.

## Suggested implementation order
1. Stable animal UID and data migration/versioning.
2. Names and owner labels.
3. Server-authoritative ownership and permissions.
4. Bounded audit history.
5. More species/UV support, then brush painting.
