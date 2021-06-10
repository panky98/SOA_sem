# get the base node image
FROM node:alpine as builder

# set the working dir for container
WORKDIR /app

# copy the json file first
COPY ./dashboard/package.json /app

# copy other project files
COPY ./dashboard/ /app

# build the folder
CMD [ "npm", "start" ]