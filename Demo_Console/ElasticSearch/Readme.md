# OS: Windows + WSL

## Prepare local Elastic docker

```shell
# prepare Elastic | https://www.elastic.co/guide/en/elasticsearch/reference/current/docker.html
docker run --name es-node01 --net elastic -p 9200:9200 -p 9300:9300 -t docker.elastic.co/elasticsearch/elasticsearch:8.5.0

# If running in Low virtual memory
# wsl -d docker-desktop
# sysctl -w vm.max_map_count=262144
# exit
# remove docker container as above running
# run above command again

# Check if elastic running successfully
curl --cacert ./http_ca.crt -u "elastic:password" https://localhost:9200

# If failing because of cert
curl --insecure -u "elastic:password" https://localhost:9200

# prepare Kibana | https://www.elastic.co/guide/en/kibana/current/docker.html
docker run --name kib-01 --net elastic -p 5601:5601 docker.elastic.co/kibana/kibana:8.5.0
```
