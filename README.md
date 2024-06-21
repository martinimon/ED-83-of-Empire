# ED-83
## Overview
This is a General Purpose discord bot. We have given it the personality of HK-47 which if you don't know is a (not so friendly) bot from the KOTOR games. Although the bot is name ED-83-of-Empire this is subject to change as the bot was originally intended to be focused around the game Edge of Empire but as it becomes more generic it may be re branded.

## Current Functionality
Currently the bot is very bare bones but here is a list of what we currently have.

### Steam API Integration
- Query the steam api to list games that we have housed in a list.
- Ability to add games to a list that we can use to query later
- Although not yet implemented does have feature page functionality


## Processes

### Router
We have designed a router that is utilised in a way that allows us to separate out the different functionality of the bot and the commands inside it. We categorise the commands by process and direct them to their associated command handlers that are responsible for processing and running the code associated with the provided command.
