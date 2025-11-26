# DemoBlazor — GitHub Pages deployment notes

This Blazor WebAssembly app is configured to be published to GitHub Pages under the repository name `PortfolioManagement`.

Key points:
- `wwwroot/index.html` base href has been set to `/PortfolioManagement/` so the app runs from the subpath on GitHub Pages.
- `wwwroot/404.html` is a copy of `index.html` to allow deep links (routes) to work correctly on GitHub Pages.
- A GitHub Actions workflow has been added at `.github/workflows/deploy.yml` which builds the project and deploys the published `wwwroot` to the `gh-pages` branch.

How to use:
1. Ensure the repository name is `PortfolioManagement` in GitHub (or change the base href if you will host it under another repo name).
2. Push a commit to `main` — the action will publish the site to the `gh-pages` branch automatically.
3. Enable GitHub Pages in repository Settings -> Pages if not already enabled (source: `gh-pages` branch).

Site URL (after enabling Pages):
https://ArbitrageScannerAPI.github.io/cases/PortfolioManagement/
