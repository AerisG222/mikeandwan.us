![](https://github.com/AerisG222/mikeandwan.us/workflows/CI/badge.svg)
[![MIT licensed](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/AerisG222/mikeandwan.us/blob/master/LICENSE.md)
[![Travis](https://img.shields.io/travis/AerisG222/mikeandwan.us.svg)](https://travis-ci.org/AerisG222/mikeandwan.us)
[![Coverity Scan](https://img.shields.io/coverity/scan/10078.svg)](https://scan.coverity.com/projects/aerisg222-mikeandwan.us)

# mikeandwan.us
mikeandwan.us is my personal website.  This has taken many forms over the years, which
first appeared at this domain in 2003.  This site serves a number of purposes:

  - Share photos and videos with my family and friends
  - Provide some tools that I occasionally find useful
  - Allows me to play with different technologies

## Current Status

As of late 2017 / early 2018, I have been working on a significant re-architecture from the original monolithic site.  This work
primiarily involves separating out the public facing site, the api, and the authentication components into their own discrete
projects.  I also spent quite a bit of time investigating Docker, and setting things up for Docker build, but at this time,
I am abandoning this approach, and will plan on hosting the different services on the primary host.  While I am interested
in getting more exposure to Docker, I am spending too much time trying to get this all to work, and would rather invest my time
in just wrapping up the latest updates.  As such, I am keeping the current docker assets in the project for reference, or if I return
to this at a later point, but do not expect to update these until then.

## Motivation

I hope that this code will offer someone either a starting point for their own site.
However, I suspect this might serve a better example of what not to do =D.  If you find
this useful, or run across ideas for improvement, I would appreciate hearing from you via
the Issues section of github - thanks!

## License

mikeandwan.us is licensed under the MIT license, see LICENSE.md for more
information.
