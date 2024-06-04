Social Network 
============

# Getting Started

## Environment

Start all containers:

```
docker-compose up -d --build --force-recreate
```

## Setup JMeter

1. Install JMeter PluginManager
Download and put into ext/lib folder
Open PluginManager and install 3 Basic Graph

2. Add Backend Listener 
Select InfluxdbBackendListenerClient
Url: http://localhost:8086/write?db=LoadTestDb


Url: http://localhost:8086/api/v2/write?org=socialnetwork&bucket=loadtest
influxdbToken: from ui

## Setup InfluxDB

Go to the page http://localhost:8086/

Fill in next value
INFLUXDB_ORGANIZATION_NAME=socialnetwork
INFLUXDB_BUCKET_NAME=loadtest

## Setup Grafana

1. Add DataSource
URL: http://influxdb:8086
Basic auth: admin
Password: adminpassword
For Flux use:
Organization: socialnetwork 
Token: from UI http://localhost:8086/ get API token
Default Bucket: loadtest

2. Add Dashboard
Download json here: https://grafana.com/grafana/dashboards/5496-apache-jmeter-dashboard-by-ubikloadpack/
and import in Grafana