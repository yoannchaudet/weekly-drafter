# Weekly Drafter

Hello my name is 🤖 **Weekly Drafter** and I am an Action. My mission is to help drafting weekly updates in the form of PRs. These weekly updates can then be posted as discussions with the help of an integration like the amazing [Announcement Drafter](https://github.com/philip-gai/announcement-drafter).

## Configuration

I need two kind of configuration.

### Files

Configuration files are expected to live in a host repository (i.e. where this Action is being invoked from).

The files should live in `.github/weekly-drafter/` and be named:

- `config.toml` The main configuration file ([example](samples/config.toml))
- `weekly.template.liquid` The template file for the weekly's content ([example](samples/weekly.template.liquid))

## Commands

I can do a few things.

### Create

- Create a pull request with a weekly update using a template
- Ping individual people (writers) that should help draft the update
- Send a Slack notification (optional)

### Remind

WIP

## Usage

Here is a full workflow example:

```yaml
name: Weekly Drafter
on:
  schedule:
    - cron: '0 13 * * THU' # Every Thursdays at 9am EDT
    - cron: '0 13 * * FRI' # Every Friday at 9am EDT

jobs:
  weekly-drafter:
    runs-on: ubuntu-latest
    steps:
      - name: Draft weekly update
        if: github.event.schedule == '0 13 * * THU'
        uses: yoannchaudet/weekly-drafter@main
        with:
          action: draft

      - name: Remind to complete weekly update
        if: github.event.schedule == '0 13 * * FRI'
        uses: yoannchaudet/weekly-drafter@main
        with:
          action: remind
```

## Contributing

Requirements:

1. [Visual Studio Code](https://code.visualstudio.com/) (preferred)
2. [.NET SDK 7.0+](https://dotnet.microsoft.com/en-us/download/visual-studio-sdks) installed

Setup:

1. Create a `.env` file at the root of the repository and initialize it as following:

   ```bash
   GITHUB_TOKEN="A Personal Access Token (classic) with repo:all permissions",
   GITHUB_REPOSITORY="owner/repo"
   ```

2. Press F5 to start debugging with the default launch command included