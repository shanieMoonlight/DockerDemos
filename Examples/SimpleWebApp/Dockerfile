# Start of a multi-stage build.
# This first stage is named 'base'. Its purpose is to hold the application's
# static files, which will be copied into the final image.
FROM nginx as base

# Copy the contents of the 'App' directory from the host machine
# into the default Nginx web root inside this container.
# This directory will contain our HTML, CSS, and JS files.
COPY ./App/ /usr/share/nginx/html

# --- Start of the second, final build stage ---

# This stage is named 'Final'. This will be the image that we actually run.
# A new Nginx image is started from scratch here.
FROM nginx as Final

# Copy the application files from the 'base' stage into a directory
# in this 'Final' image.
#
# NOTE: This line is technically incorrect for a web server, as Nginx
# will not serve files from the '/src/' directory by default.
# The files should be copied to '/usr/share/nginx/html' instead.
COPY --from=base /usr/share/nginx/html /src/

# This Dockerfile is intended to show the multi-stage copy command,
# but the final result is a non-functional web server.
# The files would be in /src/, but Nginx serves from /usr/share/nginx/html.


FROM nginx as Final
COPY --from=base /usr/share/nginx/html /src/
COPY ./App/ /usr/share/nginx/html