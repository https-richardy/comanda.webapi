"""
This is a command-line tool designed for quick authentication. It allows us to
authenticate using an email and password, retrieve an authentication token
from the Comanda API and automatically copy the token to the clipboard for
later use. The aim of this script is to simplify the process of obtaining an
authentication token for testing purposes.

Example usage:
    python3 authenticate.py -e <email> -p <password>
"""

from payloads import AuthenticationCredentials
from constants import COMANDA_LOCAL_ADDRESS

import requests
import pyperclip
import argparse

def get_authentication_token(credentials: AuthenticationCredentials) -> str:
    authentication_endpoint = f"{COMANDA_LOCAL_ADDRESS}/api/identity/authenticate"

    try:
        response = requests.post(authentication_endpoint, json=credentials.__dict__)
        response.raise_for_status()

    except requests.ConnectionError as connection_error:
        print("Connection error! Make sure the server is running.")
        raise SystemExit(-1) from connection_error

    if response.status_code == 401:
        print("Invalid credentials! Check your email and password.")
        raise SystemExit(-1)

    elif response.status_code == 200:
        response_content: dict = response.json()
        response_payload: dict = response_content.get("data")
        authentication_token: str = response_payload.get("token")

        return authentication_token

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="authenticate and get a token")

    parser.add_argument("-e", "--email", required=True, help="Email address")
    parser.add_argument("-p", "--password", required=True, help="Password")

    args = parser.parse_args()
    credentials = AuthenticationCredentials(
        email=args.email,
        password=args.password
    )

    authentication_token = get_authentication_token(credentials)
    print(f"Token copied to clipboard: {authentication_token[:15]}...")

    pyperclip.copy(authentication_token)