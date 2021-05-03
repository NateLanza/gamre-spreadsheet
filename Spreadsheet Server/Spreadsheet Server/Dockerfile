# Use alpine

FROM alpine:3.10 AS build

# The timezone library stops the docker build and waits for user input to
# select a timezone. This pauses/breaks the build. To get around this, 
# set up timezone environment variables prior to installing that library. 
# This Docker code does that. Composited from two sources:
# https://rtfm.co.ua/en/docker-configure-tzdata-and-timezone-during-build/
# https://serverfault.com/questions/949991/how-to-install-tzdata-on-a-ubuntu-docker-image

ARG DEBIAN_FRONTEND=noninteractive
ENV TZ="America/Salt Lake City"

# Install boost & necessary libraries
RUN apk --update add --no-cache \
  build-base \
  boost boost-dev

# Copy server source code in
#COPY source/ server/
COPY . server/

# Compile server source
RUN g++ -lstdc++fs -std=c++17 -o server.out server/*.cpp -lstdc++fs