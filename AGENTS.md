# Repository Guidelines — GnomesVsCamelot

## Workflow Rules (IMPORTANT)

- **Push every change immediately**: After making any edit or commit, you MUST
  push the updated commit to the remote repository right away. Do not leave
  changes unpushed.
- Default remote: `origin` (https://github.com/kdsmedia/GnomesVsCamelot.git)
- When pushing, ensure the correct upstream/branch is set (e.g. `git push -u origin <branch>`).
- Remote URL is configured with an embedded write PAT (`$GH_TOKEN`). Use:
  `git remote set-url origin "https://${GH_TOKEN}@github.com/kdsmedia/GnomesVsCamelot.git"`
  The default `$GITHUB_TOKEN` env var is metadata-read only; the write PAT is `ghp_...` provided by the user.

## Branch Layout

- `main` — **default branch on remote** (origin/HEAD), primary working branch.
- `BergusBranch` — previous default branch on remote (origin/HEAD).
- Other remote branches: `DylanBranch`, `DylanBranchReadySave`, `Master`.

## AdMob Integration

- Plugin: Google Mobile Ads Unity (v10.x) via OpenUPM (`com.google.ads.mobile`)
  + External Dependency Manager (`com.google.external-dependency-manager`).
- App ID: `ca-app-pub-6881903056221433~5097678309` (set in
  `Assets/Plugins/Android/AndroidManifest.xml` as `APPLICATION_ID` meta-data).
- Rewarded Ad Unit (Android): `ca-app-pub-6881903056221433/1098989475`
- Test rewarded ad unit (Editor/dev): `ca-app-pub-3940256099942544/5224354917`
  (auto-used when `useTestAdsInDevelopment` is true on `RewardedAdManager`).
- Manager script: `Assets/Scripts/Ads/RewardedAdManager.cs` (singleton,
  DontDestroyOnLoad, initialized from `SplashScreenManager`).
- Reward hooks (all rewarded-ad buttons use the ⚡ icon/label, NOT 📺):
  - Main Menu bonus start energy (`MainMenu.WatchAdForBonusStartEnergy`)
  - In-game +50 energy (`GameManager.WatchAdForEnergy`)
  - Game Over revive (`GameManager.WatchAdToRevive`)
- Package/identity: `com.altomedia.gnomescamelot`, company `ALTOMEDIA`,
  product `Gnomes vs Camelot`.

## Git Commit Convention

- Use conventional commit messages when possible.
- Add Co-authored-by trailer for OpenHands commits:
  `Co-authored-by: openhands <openhands@all-hands.dev>`
