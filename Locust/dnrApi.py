from locust import HttpLocust, TaskSet, task, Locust
import config
import uuid
import xml.etree.ElementTree as ET
import sys
import bs4
import os


class userBehaviour(TaskSet):
    def route(self):
        route = "SomeRoute"
        return route

    @task
    def launch_url(self):
        DNRAPI_HEADERS = {"Some headers information"

        }
        with self.client.get(self.route(), catch_response=True, headers = DNRAPI_HEADERS) as response:
            rspn = (response.content).decode("utf-8")

            if response.status_code != 200:
                response.failure(response.status_code)
            else:
                response.success()


class user(HttpLocust):
    task_set = userBehaviour
    min_wait = 5000
    max_wait = 6000
    host = "Some URL"
