FROM nginx:1.27-alpine
ENV NGINX_ENVSUBST_FILTER=^(LAB_SCHEME|LAB_HOST|LAB_PORT|MINIAPP_SCHEME|MINIAPP_HOST|MINIAPP_PORT|PORT)$
COPY default.conf.template /etc/nginx/templates/default.conf.template
EXPOSE 8080
