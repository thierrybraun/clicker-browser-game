FROM ubuntu:latest
MAINTAINER Thierry Braun <braun.thierry@outlook.com>

RUN apt-get update && apt-get -y upgrade && DEBIAN_FRONTEND=noninteractive apt-get -y install \
    apache2 php php-mysql libapache2-mod-php

#RUN a2enmod php
RUN a2enmod rewrite
RUN a2enmod ssl

# quiet logging
# RUN sed -i "s/error_reporting = .*$/error_reporting = E_ERROR | E_WARNING | E_PARSE/" /etc/php/7.0/apache2/php.ini

# Manually set up the apache environment variables
ENV APACHE_RUN_USER www-data
ENV APACHE_RUN_GROUP www-data
ENV APACHE_LOG_DIR /var/log/apache2
ENV APACHE_LOCK_DIR /var/lock/apache2
ENV APACHE_PID_FILE /var/run/apache2.pid

# Expose apache.
EXPOSE 80
EXPOSE 443

# Copy this repo into place.
ADD ./Client/build /var/www/site
ADD ./Server/api /var/www/site/api

# Update the default apache site with the config we created.
ADD ./Server/apache-config-default.conf /etc/apache2/sites-enabled/000-default.conf
ADD ./Server/apache-config-ssl.conf /etc/apache2/sites-enabled/000-ssl.conf

# By default start up apache in the foreground, override with /bin/bash for interative.
CMD /usr/sbin/apache2ctl -D FOREGROUND
